using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pathfinding
{
    [CustomEditor(typeof(Seeker))]
    [CanEditMultipleObjects]
    public class SeekerEditor : EditorBase
    {
        private static bool tagPenaltiesOpen;
        private static readonly List<Seeker> scripts = new();

        private readonly GUIContent[] exactnessLabels =
        {
            new("Node Center (Snap To Node)"), new("Original"), new("Interpolate (deprecated)"),
            new("Closest On Node Surface"), new("Node Connection")
        };

        private readonly string[] graphLabels = new string[32];

        protected override void Inspector()
        {
            base.Inspector();

            scripts.Clear();
            foreach (var script in targets) scripts.Add(script as Seeker);

            Undo.RecordObjects(targets, "Modify settings on Seeker");

            var startEndModifierProp = FindProperty("startEndModifier");
            startEndModifierProp.isExpanded =
                EditorGUILayout.Foldout(startEndModifierProp.isExpanded, startEndModifierProp.displayName);
            if (startEndModifierProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                Popup("startEndModifier.exactStartPoint", exactnessLabels, "Start Point Snapping");
                Popup("startEndModifier.exactEndPoint", exactnessLabels, "End Point Snapping");
                PropertyField("startEndModifier.addPoints", "Add Points");

                if (FindProperty("startEndModifier.exactStartPoint").enumValueIndex ==
                    (int)StartEndModifier.Exactness.Original ||
                    FindProperty("startEndModifier.exactEndPoint").enumValueIndex ==
                    (int)StartEndModifier.Exactness.Original)
                {
                    if (PropertyField("startEndModifier.useRaycasting", "Physics Raycasting"))
                    {
                        EditorGUI.indentLevel++;
                        PropertyField("startEndModifier.mask", "Layer Mask");
                        EditorGUI.indentLevel--;
                        EditorGUILayout.HelpBox(
                            "Using raycasting to snap the start/end points has largely been superseded by the 'ClosestOnNode' snapping option. It is both faster and usually closer to what you want to achieve.",
                            MessageType.Info);
                    }

                    if (PropertyField("startEndModifier.useGraphRaycasting", "Graph Raycasting"))
                        EditorGUILayout.HelpBox(
                            "Using raycasting to snap the start/end points has largely been superseded by the 'ClosestOnNode' snapping option. It is both faster and usually closer to what you want to achieve.",
                            MessageType.Info);
                }

                EditorGUI.indentLevel--;
            }

            // Make sure the AstarPath object is initialized and the graphs are loaded, this is required to be able to show graph names in the mask popup
            AstarPath.FindAstarPath();

            for (var i = 0; i < graphLabels.Length; i++)
                if (AstarPath.active == null || AstarPath.active.data.graphs == null ||
                    i >= AstarPath.active.data.graphs.Length ||
                    AstarPath.active.data.graphs[i] == null) graphLabels[i] = "Graph " + i + (i == 31 ? "+" : "");
                else
                    graphLabels[i] = AstarPath.active.data.graphs[i].name + " (graph " + i + ")";

            Mask("graphMask.value", graphLabels, "Traversable Graphs");

            tagPenaltiesOpen =
                EditorGUILayout.Foldout(tagPenaltiesOpen, new GUIContent("Tags", "Settings for each tag"));
            if (tagPenaltiesOpen)
            {
                var tagNames = AstarPath.FindTagNames();
                EditorGUI.indentLevel++;
                if (tagNames.Length != 32)
                {
                    tagNames = new string[32];
                    for (var i = 0; i < tagNames.Length; i++) tagNames[i] = "" + i;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Tag", EditorStyles.boldLabel, GUILayout.MaxWidth(120));
                for (var i = 0; i < tagNames.Length; i++)
                    EditorGUILayout.LabelField(tagNames[i], GUILayout.MaxWidth(120));

                // Make sure the arrays are all of the correct size
                for (var i = 0; i < scripts.Count; i++)
                    if (scripts[i].tagPenalties == null || scripts[i].tagPenalties.Length != tagNames.Length)
                        scripts[i].tagPenalties = new int[tagNames.Length];

                if (GUILayout.Button("Edit names", EditorStyles.miniButton)) AstarPathEditor.EditTags();
                EditorGUILayout.EndVertical();

#if !ASTAR_NoTagPenalty
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Penalty", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
                var prop = FindProperty("tagPenalties").FindPropertyRelative("Array");
                prop.Next(true);
                for (var i = 0; i < tagNames.Length; i++)
                {
                    prop.Next(false);
                    EditorGUILayout.PropertyField(prop, GUIContent.none, false, GUILayout.MinWidth(100));
                    // Penalties should not be negative
                    if (prop.intValue < 0) prop.intValue = 0;
                }

                if (GUILayout.Button("Reset all", EditorStyles.miniButton))
                    for (var i = 0; i < tagNames.Length; i++)
                    for (var j = 0; j < scripts.Count; j++)
                        scripts[j].tagPenalties[i] = 0;

                EditorGUILayout.EndVertical();
#endif

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Traversable", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
                for (var i = 0; i < tagNames.Length; i++)
                {
                    var anyFalse = false;
                    var anyTrue = false;
                    for (var j = 0; j < scripts.Count; j++)
                    {
                        var prevTraversable = ((scripts[j].traversableTags >> i) & 0x1) != 0;
                        anyTrue |= prevTraversable;
                        anyFalse |= !prevTraversable;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = anyTrue & anyFalse;
                    var newTraversable = EditorGUILayout.Toggle(anyTrue);
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                        for (var j = 0; j < scripts.Count; j++)
                            scripts[j].traversableTags = (scripts[j].traversableTags & ~(1 << i)) |
                                                         ((newTraversable ? 1 : 0) << i);
                }

                if (GUILayout.Button("Set all/none", EditorStyles.miniButton))
                    for (var j = scripts.Count - 1; j >= 0; j--)
                        scripts[j].traversableTags = (scripts[0].traversableTags & 0x1) == 0 ? -1 : 0;

                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            // Make sure we don't leak any memory
            scripts.Clear();
        }
    }
}