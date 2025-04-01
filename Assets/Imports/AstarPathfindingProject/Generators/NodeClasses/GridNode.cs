using System;
using System.Collections.Generic;
using Pathfinding.Serialization;
using UnityEngine;

namespace Pathfinding
{
    /// <summary>Node used for the GridGraph</summary>
    public class GridNode : GridNodeBase
    {
        public GridNode(AstarPath astar) : base(astar)
        {
        }

#if !ASTAR_NO_GRID_GRAPH
        private static GridGraph[] _gridGraphs = new GridGraph[0];

        public static GridGraph GetGridGraph(uint graphIndex)
        {
            return _gridGraphs[(int)graphIndex];
        }

        public static void SetGridGraph(int graphIndex, GridGraph graph)
        {
            if (_gridGraphs.Length <= graphIndex)
            {
                var gg = new GridGraph[graphIndex + 1];
                for (var i = 0; i < _gridGraphs.Length; i++) gg[i] = _gridGraphs[i];
                _gridGraphs = gg;
            }

            _gridGraphs[graphIndex] = graph;
        }

        public static void ClearGridGraph(int graphIndex, GridGraph graph)
        {
            if (graphIndex < _gridGraphs.Length && _gridGraphs[graphIndex] == graph) _gridGraphs[graphIndex] = null;
        }

        /// <summary>Internal use only</summary>
        internal ushort InternalGridFlags
        {
            get => gridFlags;
            set => gridFlags = value;
        }

        private const int GridFlagsConnectionOffset = 0;
        private const int GridFlagsConnectionBit0 = 1 << GridFlagsConnectionOffset;
        private const int GridFlagsConnectionMask = 0xFF << GridFlagsConnectionOffset;

        private const int GridFlagsEdgeNodeOffset = 10;
        private const int GridFlagsEdgeNodeMask = 1 << GridFlagsEdgeNodeOffset;

        public override bool HasConnectionsToAllEightNeighbours =>
            (InternalGridFlags & GridFlagsConnectionMask) == GridFlagsConnectionMask;

        /// <summary>
        ///     True if the node has a connection in the specified direction.
        ///     The dir parameter corresponds to directions in the grid as:
        ///     <code>
        ///         Z
        ///         |
        ///         |
        /// 
        ///      6  2  5
        ///       \ | /
        /// --  3 - X - 1  ----- X
        ///       / | \
        ///      7  0  4
        /// 
        ///         |
        ///         |
        /// </code>
        ///     See: SetConnectionInternal
        /// </summary>
        public bool HasConnectionInDirection(int dir)
        {
            return ((gridFlags >> dir) & GridFlagsConnectionBit0) != 0;
        }

        /// <summary>
        ///     True if the node has a connection in the specified direction.
        ///     Deprecated: Use HasConnectionInDirection
        /// </summary>
        [Obsolete("Use HasConnectionInDirection")]
        public bool GetConnectionInternal(int dir)
        {
            return HasConnectionInDirection(dir);
        }

        /// <summary>
        ///     Enables or disables a connection in a specified direction on the graph.
        ///     See: HasConnectionInDirection
        /// </summary>
        public void SetConnectionInternal(int dir, bool value)
        {
            // Set bit number #dir to 1 or 0 depending on #value
            unchecked
            {
                gridFlags = (ushort)((gridFlags & ~(1 << GridFlagsConnectionOffset << dir)) |
                                     ((value ? 1 : 0) << GridFlagsConnectionOffset << dir));
            }

            AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
        }

        /// <summary>
        ///     Sets the state of all grid connections.
        ///     See: SetConnectionInternal
        /// </summary>
        /// <param name="connections">
        ///     a bitmask of the connections (bit 0 is the first connection, bit 1 the second connection,
        ///     etc.).
        /// </param>
        public void SetAllConnectionInternal(int connections)
        {
            unchecked
            {
                gridFlags =
                    (ushort)((gridFlags & ~GridFlagsConnectionMask) | (connections << GridFlagsConnectionOffset));
            }

            AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
        }

        /// <summary>
        ///     Disables all grid connections from this node.
        ///     Note: Other nodes might still be able to get to this node.
        ///     Therefore it is recommended to also disable the relevant connections on adjacent nodes.
        /// </summary>
        public void ResetConnectionsInternal()
        {
            unchecked
            {
                gridFlags = (ushort)(gridFlags & ~GridFlagsConnectionMask);
            }

            AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
        }

        /// <summary>
        ///     Work in progress for a feature that required info about which nodes were at the border of the graph.
        ///     Note: This property is not functional at the moment.
        /// </summary>
        public bool EdgeNode
        {
            get => (gridFlags & GridFlagsEdgeNodeMask) != 0;
            set
            {
                unchecked
                {
                    gridFlags = (ushort)((gridFlags & ~GridFlagsEdgeNodeMask) | (value ? GridFlagsEdgeNodeMask : 0));
                }
            }
        }

        public override GridNodeBase GetNeighbourAlongDirection(int direction)
        {
            if (HasConnectionInDirection(direction))
            {
                var gg = GetGridGraph(GraphIndex);
                return gg.nodes[NodeInGridIndex + gg.neighbourOffsets[direction]];
            }

            return null;
        }

        public override void ClearConnections(bool alsoReverse)
        {
            if (alsoReverse)
                // Note: This assumes that all connections are bidirectional
                // which should hold for all grid graphs unless some custom code has been added
                for (var i = 0; i < 8; i++)
                {
                    var other = GetNeighbourAlongDirection(i) as GridNode;
                    if (other != null)
                        // Remove reverse connection. See doc for GridGraph.neighbourOffsets to see which indices are used for what.
                        other.SetConnectionInternal(i < 4 ? (i + 2) % 4 : (i - 2) % 4 + 4, false);
                }

            ResetConnectionsInternal();

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
            base.ClearConnections(alsoReverse);
#endif
        }

        public override void GetConnections(Action<GraphNode> action)
        {
            var gg = GetGridGraph(GraphIndex);

            var neighbourOffsets = gg.neighbourOffsets;
            var nodes = gg.nodes;

            for (var i = 0; i < 8; i++)
                if (HasConnectionInDirection(i))
                {
                    var other = nodes[NodeInGridIndex + neighbourOffsets[i]];
                    if (other != null) action(other);
                }

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
            base.GetConnections(action);
#endif
        }

        public override Vector3 ClosestPointOnNode(Vector3 p)
        {
            var gg = GetGridGraph(GraphIndex);

            // Convert to graph space
            p = gg.transform.InverseTransform(p);

            // Calculate graph position of this node
            var x = NodeInGridIndex % gg.width;
            var z = NodeInGridIndex / gg.width;

            // Handle the y coordinate separately
            var y = gg.transform.InverseTransform((Vector3)position).y;

            var closestInGraphSpace = new Vector3(Mathf.Clamp(p.x, x, x + 1f), y, Mathf.Clamp(p.z, z, z + 1f));

            // Convert to world space
            return gg.transform.Transform(closestInGraphSpace);
        }

        public override bool GetPortal(GraphNode other, List<Vector3> left, List<Vector3> right, bool backwards)
        {
            if (backwards) return true;

            var gg = GetGridGraph(GraphIndex);
            var neighbourOffsets = gg.neighbourOffsets;
            var nodes = gg.nodes;

            for (var i = 0; i < 4; i++)
                if (HasConnectionInDirection(i) && other == nodes[NodeInGridIndex + neighbourOffsets[i]])
                {
                    var middle = (Vector3)(position + other.position) * 0.5f;
                    var cross = Vector3.Cross(gg.collision.up, (Vector3)(other.position - position));
                    cross.Normalize();
                    cross *= gg.nodeSize * 0.5f;
                    left.Add(middle - cross);
                    right.Add(middle + cross);
                    return true;
                }

            for (var i = 4; i < 8; i++)
                if (HasConnectionInDirection(i) && other == nodes[NodeInGridIndex + neighbourOffsets[i]])
                {
                    var rClear = false;
                    var lClear = false;
                    if (HasConnectionInDirection(i - 4))
                    {
                        var n2 = nodes[NodeInGridIndex + neighbourOffsets[i - 4]];
                        if (n2.Walkable && n2.HasConnectionInDirection((i - 4 + 1) % 4)) rClear = true;
                    }

                    if (HasConnectionInDirection((i - 4 + 1) % 4))
                    {
                        var n2 = nodes[NodeInGridIndex + neighbourOffsets[(i - 4 + 1) % 4]];
                        if (n2.Walkable && n2.HasConnectionInDirection(i - 4)) lClear = true;
                    }

                    var middle = (Vector3)(position + other.position) * 0.5f;
                    var cross = Vector3.Cross(gg.collision.up, (Vector3)(other.position - position));
                    cross.Normalize();
                    cross *= gg.nodeSize * 1.4142f;
                    left.Add(middle - (lClear ? cross : Vector3.zero));
                    right.Add(middle + (rClear ? cross : Vector3.zero));
                    return true;
                }

            return false;
        }

        public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
        {
            var gg = GetGridGraph(GraphIndex);

            var neighbourOffsets = gg.neighbourOffsets;
            var nodes = gg.nodes;

            pathNode.UpdateG(path);
            handler.heap.Add(pathNode);

            var pid = handler.PathID;
            var index = NodeInGridIndex;
            for (var i = 0; i < 8; i++)
                if (HasConnectionInDirection(i))
                {
                    var other = nodes[index + neighbourOffsets[i]];
                    var otherPN = handler.GetPathNode(other);
                    if (otherPN.parent == pathNode && otherPN.pathID == pid)
                        other.UpdateRecursiveG(path, otherPN, handler);
                }

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
            base.UpdateRecursiveG(path, pathNode, handler);
#endif
        }


        public override void Open(Path path, PathNode pathNode, PathHandler handler)
        {
            var gg = GetGridGraph(GraphIndex);

            var pid = handler.PathID;

            {
                var neighbourOffsets = gg.neighbourOffsets;
                var neighbourCosts = gg.neighbourCosts;
                var nodes = gg.nodes;
                var index = NodeInGridIndex;

                for (var i = 0; i < 8; i++)
                    if (HasConnectionInDirection(i))
                    {
                        var other = nodes[index + neighbourOffsets[i]];
                        if (!path.CanTraverse(other)) continue;

                        var otherPN = handler.GetPathNode(other);

                        var tmpCost = neighbourCosts[i];

                        // Check if the other node has not yet been visited by this path
                        if (otherPN.pathID != pid)
                        {
                            otherPN.parent = pathNode;
                            otherPN.pathID = pid;

                            otherPN.cost = tmpCost;

                            otherPN.H = path.CalculateHScore(other);
                            otherPN.UpdateG(path);

                            handler.heap.Add(otherPN);
                        }
                        else
                        {
                            // Sorry for the huge number of #ifs
                            //If not we can test if the path from the current node to this one is a better one then the one already used
#if ASTAR_NO_TRAVERSAL_COST
							if (pathNode.G+tmpCost < otherPN.G)
#else
                            if (pathNode.G + tmpCost + path.GetTraversalCost(other) < otherPN.G)
#endif
                            {
                                //Debug.Log ("Path better from " + NodeIndex + " to " + otherPN.node.NodeIndex + " " + (pathNode.G+tmpCost+path.GetTraversalCost(other)) + " < " + otherPN.G);
                                otherPN.cost = tmpCost;

                                otherPN.parent = pathNode;

                                other.UpdateRecursiveG(path, otherPN, handler);
                            }
                        }
                    }
            }

#if !ASTAR_GRID_NO_CUSTOM_CONNECTIONS
            base.Open(path, pathNode, handler);
#endif
        }

        public override void SerializeNode(GraphSerializationContext ctx)
        {
            base.SerializeNode(ctx);
            ctx.SerializeInt3(position);
            ctx.writer.Write(gridFlags);
        }

        public override void DeserializeNode(GraphSerializationContext ctx)
        {
            base.DeserializeNode(ctx);
            position = ctx.DeserializeInt3();
            gridFlags = ctx.reader.ReadUInt16();
        }
#else
		public override void AddConnection (GraphNode node, uint cost) {
			throw new System.NotImplementedException();
		}

		public override void ClearConnections (bool alsoReverse) {
			throw new System.NotImplementedException();
		}

		public override void GetConnections (GraphNodeDelegate del) {
			throw new System.NotImplementedException();
		}

		public override void Open (Path path, PathNode pathNode, PathHandler handler) {
			throw new System.NotImplementedException();
		}

		public override void RemoveConnection (GraphNode node) {
			throw new System.NotImplementedException();
		}
#endif
    }
}