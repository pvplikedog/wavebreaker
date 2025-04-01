using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Pathfinding.WindowsStore;
using UnityEditor;
using UnityEngine;
using Guid = Pathfinding.Util.Guid;
using Object = UnityEngine.Object;
#if NETFX_CORE
using System.Linq;
using WinRTLegacy;
#endif

namespace Pathfinding.Serialization
{
    public class JsonMemberAttribute : Attribute
    {
    }

    public class JsonOptInAttribute : Attribute
    {
    }

    /// <summary>
    ///     A very tiny json serializer.
    ///     It is not supposed to have lots of features, it is only intended to be able to serialize graph settings
    ///     well enough.
    /// </summary>
    public class TinyJsonSerializer
    {
        private static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        private StringBuilder output = new();

        private readonly Dictionary<Type, Action<object>> serializers = new();

        private TinyJsonSerializer()
        {
            serializers[typeof(float)] = v => output.Append(((float)v).ToString("R", invariantCulture));
            serializers[typeof(bool)] = v => output.Append((bool)v ? "true" : "false");
            serializers[typeof(Version)] = serializers[typeof(uint)] = serializers[typeof(int)] = v => output.Append(v);
            serializers[typeof(string)] = v => output.AppendFormat("\"{0}\"", v.ToString().Replace("\"", "\\\""));
            serializers[typeof(Vector2)] = v => output.AppendFormat("{{ \"x\": {0}, \"y\": {1} }}",
                ((Vector2)v).x.ToString("R", invariantCulture), ((Vector2)v).y.ToString("R", invariantCulture));
            serializers[typeof(Vector3)] = v => output.AppendFormat("{{ \"x\": {0}, \"y\": {1}, \"z\": {2} }}",
                ((Vector3)v).x.ToString("R", invariantCulture), ((Vector3)v).y.ToString("R", invariantCulture),
                ((Vector3)v).z.ToString("R", invariantCulture));
            serializers[typeof(Guid)] = v => output.AppendFormat("{{ \"value\": \"{0}\" }}", v);
            serializers[typeof(LayerMask)] =
                v => output.AppendFormat("{{ \"value\": {0} }}", ((int)(LayerMask)v).ToString());
        }

        public static void Serialize(object obj, StringBuilder output)
        {
            new TinyJsonSerializer
            {
                output = output
            }.Serialize(obj);
        }

        private void Serialize(object obj)
        {
            if (obj == null)
            {
                output.Append("null");
                return;
            }

            var type = obj.GetType();
            var typeInfo = WindowsStoreCompatibility.GetTypeInfo(type);
            if (serializers.ContainsKey(type))
            {
                serializers[type](obj);
            }
            else if (typeInfo.IsEnum)
            {
                output.Append('"' + obj.ToString() + '"');
            }
            else if (obj is IList)
            {
                output.Append("[");
                var arr = obj as IList;
                for (var i = 0; i < arr.Count; i++)
                {
                    if (i != 0)
                        output.Append(", ");
                    Serialize(arr[i]);
                }

                output.Append("]");
            }
            else if (obj is Object)
            {
                SerializeUnityObject(obj as Object);
            }
            else
            {
#if NETFX_CORE
				var optIn = typeInfo.CustomAttributes.Any(attr => attr.GetType() == typeof(JsonOptInAttribute));
#else
                var optIn = typeInfo.GetCustomAttributes(typeof(JsonOptInAttribute), true).Length > 0;
#endif
                output.Append("{");
                var earlier = false;

                while (true)
                {
#if NETFX_CORE
					var fields = typeInfo.DeclaredFields.Where(f => !f.IsStatic).ToArray();
#else
                    var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#endif
                    foreach (var field in fields)
                    {
                        if (field.DeclaringType != type) continue;
                        if ((!optIn && field.IsPublic) ||
#if NETFX_CORE
							field.CustomAttributes.Any(attr => attr.GetType() == typeof(JsonMemberAttribute))
#else
                            field.GetCustomAttributes(typeof(JsonMemberAttribute), true).Length > 0
#endif
                           )
                        {
                            if (earlier) output.Append(", ");

                            earlier = true;
                            output.AppendFormat("\"{0}\": ", field.Name);
                            Serialize(field.GetValue(obj));
                        }
                    }

#if NETFX_CORE
					typeInfo = typeInfo.BaseType;
					if (typeInfo == null) break;
#else
                    type = type.BaseType;
                    if (type == null) break;
#endif
                }

                output.Append("}");
            }
        }

        private void QuotedField(string name, string contents)
        {
            output.AppendFormat("\"{0}\": \"{1}\"", name, contents);
        }

        private void SerializeUnityObject(Object obj)
        {
            // Note that a unityengine can be destroyed as well
            if (obj == null)
            {
                Serialize(null);
                return;
            }

            output.Append("{");
            var path = obj.name;
#if UNITY_EDITOR
            // Figure out the path of the object relative to a Resources folder.
            // In a standalone player this cannot be done unfortunately, so we will assume it is at the top level in the Resources folder.
            // Fortunately it should be extremely rare to have to serialize references to unity objects in a standalone player.
            var realPath = AssetDatabase.GetAssetPath(obj);
            var match = Regex.Match(realPath, @"Resources/(.*?)(\.\w+)?$");
            if (match != null) path = match.Groups[1].Value;
#endif
            QuotedField("Name", path);
            output.Append(", ");
            QuotedField("Type", obj.GetType().FullName);

            //Write scene path if the object is a Component or GameObject
            var component = obj as Component;
            var go = obj as GameObject;

            if (component != null || go != null)
            {
                if (component != null && go == null) go = component.gameObject;

                var helper = go.GetComponent<UnityReferenceHelper>();

                if (helper == null)
                {
                    Debug.Log("Adding UnityReferenceHelper to Unity Reference '" + obj.name + "'");
                    helper = go.AddComponent<UnityReferenceHelper>();
                }

                //Make sure it has a unique GUID
                helper.Reset();
                output.Append(", ");
                QuotedField("GUID", helper.GetGUID());
            }

            output.Append("}");
        }
    }

    /// <summary>
    ///     A very tiny json deserializer.
    ///     It is not supposed to have lots of features, it is only intended to be able to deserialize graph settings
    ///     well enough. Not much validation of the input is done.
    /// </summary>
    public class TinyJsonDeserializer
    {
        private static readonly NumberFormatInfo numberFormat = NumberFormatInfo.InvariantInfo;

        private readonly StringBuilder builder = new();
        private GameObject contextRoot;
        private TextReader reader;

        /// <summary>
        ///     Deserializes an object of the specified type.
        ///     Will load all fields into the populate object if it is set (only works for classes).
        /// </summary>
        public static object Deserialize(string text, Type type, object populate = null, GameObject contextRoot = null)
        {
            return new TinyJsonDeserializer
            {
                reader = new StringReader(text),
                contextRoot = contextRoot
            }.Deserialize(type, populate);
        }

        /// <summary>
        ///     Deserializes an object of type tp.
        ///     Will load all fields into the populate object if it is set (only works for classes).
        /// </summary>
        private object Deserialize(Type tp, object populate = null)
        {
            var tpInfo = WindowsStoreCompatibility.GetTypeInfo(tp);

            if (tpInfo.IsEnum) return Enum.Parse(tp, EatField());

            if (TryEat('n'))
            {
                Eat("ull");
                TryEat(',');
                return null;
            }

            if (Equals(tp, typeof(float))) return float.Parse(EatField(), numberFormat);

            if (Equals(tp, typeof(int))) return int.Parse(EatField(), numberFormat);

            if (Equals(tp, typeof(uint))) return uint.Parse(EatField(), numberFormat);

            if (Equals(tp, typeof(bool))) return bool.Parse(EatField());

            if (Equals(tp, typeof(string))) return EatField();

            if (Equals(tp, typeof(Version))) return new Version(EatField());

            if (Equals(tp, typeof(Vector2)))
            {
                Eat("{");
                var result = new Vector2();
                EatField();
                result.x = float.Parse(EatField(), numberFormat);
                EatField();
                result.y = float.Parse(EatField(), numberFormat);
                Eat("}");
                return result;
            }

            if (Equals(tp, typeof(Vector3)))
            {
                Eat("{");
                var result = new Vector3();
                EatField();
                result.x = float.Parse(EatField(), numberFormat);
                EatField();
                result.y = float.Parse(EatField(), numberFormat);
                EatField();
                result.z = float.Parse(EatField(), numberFormat);
                Eat("}");
                return result;
            }

            if (Equals(tp, typeof(Guid)))
            {
                Eat("{");
                EatField();
                var result = Guid.Parse(EatField());
                Eat("}");
                return result;
            }

            if (Equals(tp, typeof(LayerMask)))
            {
                Eat("{");
                EatField();
                var result = (LayerMask)int.Parse(EatField());
                Eat("}");
                return result;
            }

            if (Equals(tp, typeof(List<string>)))
            {
                IList result = new List<string>();

                Eat("[");
                while (!TryEat(']'))
                {
                    result.Add(Deserialize(typeof(string)));
                    TryEat(',');
                }

                return result;
            }

            if (tpInfo.IsArray)
            {
                var ls = new List<object>();
                Eat("[");
                while (!TryEat(']'))
                {
                    ls.Add(Deserialize(tp.GetElementType()));
                    TryEat(',');
                }

                var arr = Array.CreateInstance(tp.GetElementType(), ls.Count);
                ls.ToArray().CopyTo(arr, 0);
                return arr;
            }

            if (Equals(tp, typeof(Mesh)) || Equals(tp, typeof(Texture2D)) || Equals(tp, typeof(Transform)) ||
                Equals(tp, typeof(GameObject))) return DeserializeUnityObject();

            var obj = populate ?? Activator.CreateInstance(tp);
            Eat("{");
            while (!TryEat('}'))
            {
                var name = EatField();
                var tmpType = tp;
                FieldInfo field = null;
                while (field == null && tmpType != null)
                {
                    field = tmpType.GetField(name,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    tmpType = tmpType.BaseType;
                }

                if (field == null)
                    SkipFieldData();
                else
                    field.SetValue(obj, Deserialize(field.FieldType));
                TryEat(',');
            }

            return obj;
        }

        private Object DeserializeUnityObject()
        {
            Eat("{");
            var result = DeserializeUnityObjectInner();
            Eat("}");
            return result;
        }

        private Object DeserializeUnityObjectInner()
        {
            // Ignore InstanceID field (compatibility only)
            var fieldName = EatField();

            if (fieldName == "InstanceID")
            {
                EatField();
                fieldName = EatField();
            }

            if (fieldName != "Name") throw new Exception("Expected 'Name' field");
            var name = EatField();

            if (name == null) return null;

            if (EatField() != "Type") throw new Exception("Expected 'Type' field");
            var typename = EatField();

            // Remove assembly information
            if (typename.IndexOf(',') != -1) typename = typename.Substring(0, typename.IndexOf(','));

            // Note calling through assembly is more stable on e.g WebGL
            var type = WindowsStoreCompatibility.GetTypeInfo(typeof(AstarPath)).Assembly.GetType(typename);
            type = type ?? WindowsStoreCompatibility.GetTypeInfo(typeof(Transform)).Assembly.GetType(typename);

            if (Equals(type, null))
            {
                Debug.LogError("Could not find type '" + typename + "'. Cannot deserialize Unity reference");
                return null;
            }

            // Check if there is another field there
            EatWhitespace();
            if ((char)reader.Peek() == '"')
            {
                if (EatField() != "GUID") throw new Exception("Expected 'GUID' field");
                var guid = EatField();

                if (contextRoot != null)
                    foreach (var helper in contextRoot.GetComponentsInChildren<UnityReferenceHelper>(true))
                        if (helper.GetGUID() == guid)
                        {
                            if (Equals(type, typeof(GameObject))) return helper.gameObject;

                            return helper.GetComponent(type);
                        }

#if UNITY_2020_1_OR_NEWER
                foreach (var helper in Object.FindObjectsOfType<UnityReferenceHelper>(true))
#else
				foreach (var helper in UnityEngine.Object.FindObjectsOfType<UnityReferenceHelper>())
#endif
                    if (helper.GetGUID() == guid)
                    {
                        if (Equals(type, typeof(GameObject))) return helper.gameObject;

                        return helper.GetComponent(type);
                    }
            }

            // Note: calling LoadAll with an empty string will make it load the whole resources folder, which is probably a bad idea.
            if (!string.IsNullOrEmpty(name))
            {
                // Try to load from resources
                var objs = Resources.LoadAll(name, type);

                for (var i = 0; i < objs.Length; i++)
                    if (objs[i].name == name || objs.Length == 1)
                        return objs[i];
            }

            return null;
        }

        private void EatWhitespace()
        {
            while (char.IsWhiteSpace((char)reader.Peek()))
                reader.Read();
        }

        private void Eat(string s)
        {
            EatWhitespace();
            for (var i = 0; i < s.Length; i++)
            {
                var c = (char)reader.Read();
                if (c != s[i])
                    throw new Exception("Expected '" + s[i] + "' found '" + c + "'\n\n..." + reader.ReadLine());
            }
        }

        private string EatUntil(string c, bool inString)
        {
            builder.Length = 0;
            var escape = false;
            while (true)
            {
                var readInt = reader.Peek();
                if (!escape && (char)readInt == '"') inString = !inString;

                var readChar = (char)readInt;
                if (readInt == -1) throw new Exception("Unexpected EOF");

                if (!escape && readChar == '\\')
                {
                    escape = true;
                    reader.Read();
                }
                else if (!inString && c.IndexOf(readChar) != -1)
                {
                    break;
                }
                else
                {
                    builder.Append(readChar);
                    reader.Read();
                    escape = false;
                }
            }

            return builder.ToString();
        }

        private bool TryEat(char c)
        {
            EatWhitespace();
            if ((char)reader.Peek() == c)
            {
                reader.Read();
                return true;
            }

            return false;
        }

        private string EatField()
        {
            var result = EatUntil("\",}]", TryEat('"'));

            TryEat('\"');
            TryEat(':');
            TryEat(',');
            return result;
        }

        private void SkipFieldData()
        {
            var indent = 0;

            while (true)
            {
                EatUntil(",{}[]", false);
                var last = (char)reader.Peek();

                switch (last)
                {
                    case '{':
                    case '[':
                        indent++;
                        break;
                    case '}':
                    case ']':
                        indent--;
                        if (indent < 0) return;
                        break;
                    case ',':
                        if (indent == 0)
                        {
                            reader.Read();
                            return;
                        }

                        break;
                    default:
                        throw new Exception("Should not reach this part");
                }

                reader.Read();
            }
        }
    }
}