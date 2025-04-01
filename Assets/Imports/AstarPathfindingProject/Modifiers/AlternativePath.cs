using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Pathfinding
{
    [AddComponentMenu("Pathfinding/Modifiers/Alternative Path")]
    [Serializable]
    /// <summary>
    /// Applies penalty to the paths it processes telling other units to avoid choosing the same path.
    ///
    /// Note that this might not work properly if penalties are modified by other actions as well (e.g graph update objects which reset the penalty to zero).
    /// It will only work when all penalty modifications are relative, i.e adding or subtracting penalties, but not when setting penalties
    /// to specific values.
    ///
    /// When destroyed, it will correctly remove any added penalty.
    ///
    /// \ingroup modifiers
    /// </summary>
    [HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_alternative_path.php")]
    public class AlternativePath : MonoModifier
    {
        /// <summary>How much penalty (weight) to apply to nodes</summary>
        public int penalty = 1000;

        /// <summary>Max number of nodes to skip in a row</summary>
        public int randomStep = 10;

        /// <summary>A random object</summary>
        private readonly Random rnd = new();

        private bool destroyed;

        /// <summary>The previous path</summary>
        private List<GraphNode> prevNodes = new();

        /// <summary>The previous penalty used. Stored just in case it changes during operation</summary>
        private int prevPenalty;

        public override int Order => 10;

        protected void OnDestroy()
        {
            destroyed = true;
            ClearOnDestroy();
        }
#if UNITY_EDITOR
        [MenuItem("CONTEXT/Seeker/Add Alternative Path Modifier")]
        public static void AddComp(MenuCommand command)
        {
            (command.context as Component).gameObject.AddComponent(typeof(AlternativePath));
        }
#endif

        public override void Apply(Path p)
        {
            if (this == null) return;

            ApplyNow(p.path);
        }

        private void ClearOnDestroy()
        {
            InversePrevious();
        }

        private void InversePrevious()
        {
            // Remove previous penalty
            if (prevNodes != null)
            {
                var warnPenalties = false;
                for (var i = 0; i < prevNodes.Count; i++)
                    if (prevNodes[i].Penalty < prevPenalty)
                    {
                        warnPenalties = true;
                        prevNodes[i].Penalty = 0;
                    }
                    else
                    {
                        prevNodes[i].Penalty = (uint)(prevNodes[i].Penalty - prevPenalty);
                    }

                if (warnPenalties)
                    Debug.LogWarning(
                        "Penalty for some nodes has been reset while the AlternativePath modifier was active (possibly because of a graph update). Some penalties might be incorrect (they may be lower than expected for the affected nodes)");
            }
        }

        private void ApplyNow(List<GraphNode> nodes)
        {
            InversePrevious();
            prevNodes.Clear();

            if (destroyed) return;

            if (nodes != null)
            {
                var rndStart = rnd.Next(randomStep);
                for (var i = rndStart; i < nodes.Count; i += rnd.Next(1, randomStep))
                {
                    nodes[i].Penalty = (uint)(nodes[i].Penalty + penalty);
                    prevNodes.Add(nodes[i]);
                }
            }

            prevPenalty = penalty;
        }
    }
}