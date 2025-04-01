using System;
using Pathfinding.Util;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pathfinding
{
	/// <summary>
	///     Connects two nodes with a direct connection.
	///     It is not possible to detect this link when following a path (which may be good or bad), for that you can use
	///     NodeLink2.
	///     [Open online documentation to see images]
	///     See: editing-graphs (view in online documentation for working links)
	/// </summary>
	[AddComponentMenu("Pathfinding/Link")]
    [HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_node_link.php")]
    public class NodeLink : GraphModifier
    {
        /// <summary>End position of the link</summary>
        public Transform end;

        /// <summary>
        ///     The connection will be this times harder/slower to traverse.
        ///     Note that values lower than one will not always make the pathfinder choose this path instead of another path even
        ///     though this one should
        ///     lead to a lower total cost unless you also adjust the Heuristic Scale in A* Inspector -> Settings -> Pathfinding or
        ///     disable the heuristic altogether.
        /// </summary>
        public float costFactor = 1.0f;

        /// <summary>Make a one-way connection</summary>
        public bool oneWay;

        /// <summary>Delete existing connection instead of adding one</summary>
        public bool deleteConnection;

        public Transform Start => transform;

        public Transform End => end;

        public void OnDrawGizmos()
        {
            if (Start == null || End == null) return;

            Draw.Gizmos.Bezier(Start.position, End.position, deleteConnection ? Color.red : Color.green);
        }

        public override void OnPostScan()
        {
            if (AstarPath.active.isScanning)
                InternalOnPostScan();
            else
                AstarPath.active.AddWorkItem(new AstarWorkItem(force =>
                {
                    InternalOnPostScan();
                    return true;
                }));
        }

        public void InternalOnPostScan()
        {
            Apply();
        }

        public override void OnGraphsPostUpdate()
        {
            if (!AstarPath.active.isScanning)
                AstarPath.active.AddWorkItem(new AstarWorkItem(force =>
                {
                    InternalOnPostScan();
                    return true;
                }));
        }

        public virtual void Apply()
        {
            if (Start == null || End == null || AstarPath.active == null) return;

            var startNode = AstarPath.active.GetNearest(Start.position).node;
            var endNode = AstarPath.active.GetNearest(End.position).node;

            if (startNode == null || endNode == null) return;


            if (deleteConnection)
            {
                startNode.RemoveConnection(endNode);
                if (!oneWay)
                    endNode.RemoveConnection(startNode);
            }
            else
            {
                var cost = (uint)Math.Round((startNode.position - endNode.position).costMagnitude * costFactor);

                startNode.AddConnection(endNode, cost);
                if (!oneWay)
                    endNode.AddConnection(startNode, cost);
            }
        }

#if UNITY_EDITOR
        [MenuItem("Edit/Pathfinding/Link Pair %&l")]
        public static void LinkObjects()
        {
            var tfs = Selection.transforms;
            if (tfs.Length == 2) LinkObjects(tfs[0], tfs[1], false);
            SceneView.RepaintAll();
        }

        [MenuItem("Edit/Pathfinding/Unlink Pair %&u")]
        public static void UnlinkObjects()
        {
            var tfs = Selection.transforms;
            if (tfs.Length == 2) LinkObjects(tfs[0], tfs[1], true);
            SceneView.RepaintAll();
        }

        [MenuItem("Edit/Pathfinding/Delete Links on Selected %&b")]
        public static void DeleteLinks()
        {
            var tfs = Selection.transforms;
            for (var i = 0; i < tfs.Length; i++)
            {
                var conns = tfs[i].GetComponents<NodeLink>();
                for (var j = 0; j < conns.Length; j++) DestroyImmediate(conns[j]);
            }

            SceneView.RepaintAll();
        }

        public static void LinkObjects(Transform a, Transform b, bool removeConnection)
        {
            NodeLink connecting = null;

            var conns = a.GetComponents<NodeLink>();
            for (var i = 0; i < conns.Length; i++)
                if (conns[i].end == b)
                {
                    connecting = conns[i];
                    break;
                }

            conns = b.GetComponents<NodeLink>();
            for (var i = 0; i < conns.Length; i++)
                if (conns[i].end == a)
                {
                    connecting = conns[i];
                    break;
                }

            if (removeConnection)
            {
                if (connecting != null) DestroyImmediate(connecting);
            }
            else
            {
                if (connecting == null)
                {
                    connecting = a.gameObject.AddComponent<NodeLink>();
                    connecting.end = b;
                }
                else
                {
                    connecting.deleteConnection = !connecting.deleteConnection;
                }
            }
        }
#endif
    }
}