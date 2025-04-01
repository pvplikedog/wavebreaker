using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Pathfinding
{
    /// <summary>Helper for creating editors</summary>
    [CustomEditor(typeof(VersionedMonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class EditorBase : Editor
    {
        private static Dictionary<string, string> cachedTooltips;
        private static Dictionary<string, string> cachedURLs;

        private static readonly GUIContent content = new();
        private static readonly GUIContent showInDocContent = new("Show in online documentation", "");
        private static readonly GUILayoutOption[] noOptions = new GUILayoutOption[0];
        public static Func<string> getDocumentationURL;
        private readonly Dictionary<string, string> localTooltips = new();
        private readonly Dictionary<string, SerializedProperty> props = new();

        protected virtual void OnEnable()
        {
            foreach (var target in targets)
                if (target != null)
                    (target as IVersionedMonoBehaviourInternal).UpgradeFromUnityThread();
        }

        private static void LoadMeta()
        {
            if (cachedTooltips == null)
            {
                var filePath = EditorResourceHelper.editorAssets + "/tooltips.tsv";

                try
                {
                    cachedURLs = File.ReadAllLines(filePath).Select(l => l.Split('\t')).Where(l => l.Length == 2)
                        .ToDictionary(l => l[0], l => l[1]);
                    cachedTooltips = new Dictionary<string, string>();
                }
                catch
                {
                    cachedURLs = new Dictionary<string, string>();
                    cachedTooltips = new Dictionary<string, string>();
                }
            }
        }

        private static string FindURL(Type type, string path)
        {
            // Find the correct type if the path was not an immediate member of #type
            while (true)
            {
                var index = path.IndexOf('.');
                if (index == -1) break;
                var fieldName = path.Substring(0, index);
                var remaining = path.Substring(index + 1);
                var field = type.GetField(fieldName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    type = field.FieldType;
                    path = remaining;
                }
                else
                {
                    // Could not find the correct field
                    return null;
                }
            }

            // Find a documentation entry for the field, fall back to parent classes if necessary
            while (type != null)
            {
                var url = FindURL(type.FullName + "." + path);
                if (url != null) return url;
                type = type.BaseType;
            }

            return null;
        }

        private static string FindURL(string path)
        {
            LoadMeta();
            string url;
            cachedURLs.TryGetValue(path, out url);
            return url;
        }

        private static string FindTooltip(string path)
        {
            LoadMeta();

            string tooltip;
            cachedTooltips.TryGetValue(path, out tooltip);
            return tooltip;
        }

        private string FindLocalTooltip(string path)
        {
            string result;

            if (!localTooltips.TryGetValue(path, out result))
            {
                var fullPath = target.GetType().Name + "." + path;
                result = localTooltips[path] = FindTooltip(fullPath);
            }

            return result;
        }

        public sealed override void OnInspectorGUI()
        {
            EditorGUI.indentLevel = 0;
            serializedObject.Update();
            try
            {
                Inspector();
            }
            catch (Exception e)
            {
                Debug.LogException(e, target);
            }

            serializedObject.ApplyModifiedProperties();
            if (targets.Length == 1 && (target as MonoBehaviour).enabled)
            {
                var attr = target.GetType().GetCustomAttributes(typeof(UniqueComponentAttribute), true);
                for (var i = 0; i < attr.Length; i++)
                {
                    var tag = (attr[i] as UniqueComponentAttribute).tag;
                    foreach (var other in (target as MonoBehaviour).GetComponents<MonoBehaviour>())
                    {
                        if (!other.enabled || other == target) continue;
                        if (other.GetType().GetCustomAttributes(typeof(UniqueComponentAttribute), true)
                            .Select(c => (c as UniqueComponentAttribute).tag == tag).Any())
                            EditorGUILayout.HelpBox(
                                "This component and " + other.GetType().Name + " cannot be used at the same time",
                                MessageType.Warning);
                    }
                }
            }
        }

        protected virtual void Inspector()
        {
            // Basically the same as DrawDefaultInspector, but with tooltips
            var enterChildren = true;

            for (var prop = serializedObject.GetIterator(); prop.NextVisible(enterChildren); enterChildren = false)
                PropertyField(prop.propertyPath);
        }

        protected SerializedProperty FindProperty(string name)
        {
            if (!props.TryGetValue(name, out var res)) res = props[name] = serializedObject.FindProperty(name);
            if (res == null) throw new ArgumentException(name);
            return res;
        }

        protected void Section(string label)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        protected void FloatField(string propertyPath, string label = null, string tooltip = null,
            float min = float.NegativeInfinity, float max = float.PositiveInfinity)
        {
            PropertyField(propertyPath, label, tooltip);
            Clamp(propertyPath, min, max);
        }

        protected void FloatField(SerializedProperty prop, string label = null, string tooltip = null,
            float min = float.NegativeInfinity, float max = float.PositiveInfinity)
        {
            PropertyField(prop, label, tooltip);
            Clamp(prop, min, max);
        }

        protected bool PropertyField(string propertyPath, string label = null, string tooltip = null)
        {
            return PropertyField(FindProperty(propertyPath), label, tooltip, propertyPath);
        }

        protected bool PropertyField(SerializedProperty prop, string label = null, string tooltip = null)
        {
            return PropertyField(prop, label, tooltip, prop.propertyPath);
        }

        private bool PropertyField(SerializedProperty prop, string label, string tooltip, string propertyPath)
        {
            content.text = label ?? prop.displayName;
            content.tooltip = tooltip ?? FindTooltip(propertyPath);
            var contextClick = IsContextClick();
            EditorGUILayout.PropertyField(prop, content, true, noOptions);
            // Disable context clicking on arrays (as Unity has its own very useful context menu for the array elements)
            if (contextClick && !prop.isArray && Event.current.type == EventType.Used)
                CaptureContextClick(propertyPath);
            return prop.propertyType == SerializedPropertyType.Boolean
                ? !prop.hasMultipleDifferentValues && prop.boolValue
                : true;
        }

        private bool IsContextClick()
        {
            // Capturing context clicks turned out to be a bad idea.
            // It prevents things like reverting to prefab values and other nice things.
            return false;
            // return Event.current.type == EventType.ContextClick;
        }

        private void CaptureContextClick(string propertyPath)
        {
            var url = FindURL(target.GetType(), propertyPath);

            if (url != null && getDocumentationURL != null)
            {
                Event.current.Use();
                var menu = new GenericMenu();
                menu.AddItem(showInDocContent, false, () => Application.OpenURL(getDocumentationURL() + url));
                menu.ShowAsContext();
            }
        }

        protected void Popup(string propertyPath, GUIContent[] options, string label = null)
        {
            var prop = FindProperty(propertyPath);

            content.text = label ?? prop.displayName;
            content.tooltip = FindTooltip(propertyPath);
            var contextClick = IsContextClick();
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
            var newVal = EditorGUILayout.Popup(content,
                prop.propertyType == SerializedPropertyType.Enum ? prop.enumValueIndex : prop.intValue, options);
            if (EditorGUI.EndChangeCheck())
            {
                if (prop.propertyType == SerializedPropertyType.Enum) prop.enumValueIndex = newVal;
                else prop.intValue = newVal;
            }

            EditorGUI.showMixedValue = false;
            if (contextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                CaptureContextClick(propertyPath);
        }

        protected void Mask(string propertyPath, string[] options, string label = null)
        {
            var prop = FindProperty(propertyPath);

            content.text = label ?? prop.displayName;
            content.tooltip = FindTooltip(propertyPath);
            var contextClick = IsContextClick();
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
            var newVal = EditorGUILayout.MaskField(content, prop.intValue, options);
            if (EditorGUI.EndChangeCheck()) prop.intValue = newVal;
            EditorGUI.showMixedValue = false;
            if (contextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                CaptureContextClick(propertyPath);
        }

        protected void IntSlider(string propertyPath, int left, int right)
        {
            var contextClick = IsContextClick();
            var prop = FindProperty(propertyPath);

            content.text = prop.displayName;
            content.tooltip = FindTooltip(propertyPath);
            EditorGUILayout.IntSlider(prop, left, right, content, noOptions);
            if (contextClick && Event.current.type == EventType.Used) CaptureContextClick(propertyPath);
        }

        protected void Slider(string propertyPath, float left, float right)
        {
            var contextClick = IsContextClick();
            var prop = FindProperty(propertyPath);

            content.text = prop.displayName;
            content.tooltip = FindTooltip(propertyPath);
            EditorGUILayout.Slider(prop, left, right, content, noOptions);
            if (contextClick && Event.current.type == EventType.Used) CaptureContextClick(propertyPath);
        }

        protected void Clamp(SerializedProperty prop, float min, float max = float.PositiveInfinity)
        {
            if (!prop.hasMultipleDifferentValues) prop.floatValue = Mathf.Clamp(prop.floatValue, min, max);
        }

        protected void Clamp(string name, float min, float max = float.PositiveInfinity)
        {
            Clamp(FindProperty(name), min, max);
        }

        protected void ClampInt(string name, int min, int max = int.MaxValue)
        {
            var prop = FindProperty(name);

            if (!prop.hasMultipleDifferentValues) prop.intValue = Mathf.Clamp(prop.intValue, min, max);
        }
    }
}