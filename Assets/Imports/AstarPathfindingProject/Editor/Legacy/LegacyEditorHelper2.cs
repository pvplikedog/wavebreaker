using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pathfinding.Legacy
{
    public static class LegacyEditorHelper
    {
        public static void UpgradeDialog(Object[] targets, Type upgradeType)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var gui = EditorGUIUtility.IconContent("console.warnicon");
            gui.text =
                "You are using the compatibility version of this component. It is recommended that you upgrade to the newer version. This may change the component's behavior.";
            EditorGUILayout.LabelField(GUIContent.none, gui, EditorStyles.wordWrappedMiniLabel);
            if (GUILayout.Button("Upgrade"))
            {
                Undo.RecordObjects(targets.Select(s => (s as Component).gameObject).ToArray(),
                    "Upgrade from Legacy Component");
                foreach (var tg in targets)
                {
                    var comp = tg as Component;
                    var components = comp.gameObject.GetComponents<Component>();
                    var index = Array.IndexOf(components, comp);
                    var newRVO = Undo.AddComponent(comp.gameObject, upgradeType);
                    foreach (var field in newRVO.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                        field.SetValue(newRVO, field.GetValue(comp));
                    Undo.DestroyObjectImmediate(comp);
                    for (var i = components.Length - 1; i > index; i--) ComponentUtility.MoveComponentUp(newRVO);
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}