using Pathfinding.Util;
using UnityEditor;
using UnityEngine;

namespace Pathfinding
{
    [CustomEditor(typeof(AnimationLink))]
    public class AnimationLinkEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = target as AnimationLink;

            EditorGUI.BeginDisabledGroup(script.EndTransform == null);
            if (GUILayout.Button("Autoposition Endpoint"))
            {
                var buffer = ListPool<Vector3>.Claim();
                Vector3 endpos;
                script.CalculateOffsets(buffer, out endpos);
                script.EndTransform.position = endpos;
                ListPool<Vector3>.Release(buffer);
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}