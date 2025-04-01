using UnityEngine;

namespace Pathfinding
{
	/// <summary>
	///     Pruning of recast navmesh regions.
	///     A RelevantGraphSurface component placed in the scene specifies that
	///     the navmesh region it is inside should be included in the navmesh.
	///     See: Pathfinding.RecastGraph.relevantGraphSurfaceMode
	/// </summary>
	[AddComponentMenu("Pathfinding/Navmesh/RelevantGraphSurface")]
    [HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_relevant_graph_surface.php")]
    public class RelevantGraphSurface : VersionedMonoBehaviour
    {
        public float maxRange = 1;

        public Vector3 Position { get; private set; }

        public RelevantGraphSurface Next { get; private set; }

        public RelevantGraphSurface Prev { get; private set; }

        public static RelevantGraphSurface Root { get; private set; }

        private void OnEnable()
        {
            UpdatePosition();
            if (Root == null)
            {
                Root = this;
            }
            else
            {
                Next = Root;
                Root.Prev = this;
                Root = this;
            }
        }

        private void OnDisable()
        {
            if (Root == this)
            {
                Root = Next;
                if (Root != null) Root.Prev = null;
            }
            else
            {
                if (Prev != null) Prev.Next = Next;
                if (Next != null) Next.Prev = Prev;
            }

            Prev = null;
            Next = null;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = new Color(57 / 255f, 211 / 255f, 46 / 255f, 0.4f);
            Gizmos.DrawLine(transform.position - Vector3.up * maxRange, transform.position + Vector3.up * maxRange);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(57 / 255f, 211 / 255f, 46 / 255f);
            Gizmos.DrawLine(transform.position - Vector3.up * maxRange, transform.position + Vector3.up * maxRange);
        }

        public void UpdatePosition()
        {
            Position = transform.position;
        }

        /// <summary>
        ///     Updates the positions of all relevant graph surface components.
        ///     Required to be able to use the position property reliably.
        /// </summary>
        public static void UpdateAllPositions()
        {
            var c = Root;

            while (c != null)
            {
                c.UpdatePosition();
                c = c.Next;
            }
        }

        public static void FindAllGraphSurfaces()
        {
            var srf = FindObjectsOfType(typeof(RelevantGraphSurface)) as RelevantGraphSurface[];

            for (var i = 0; i < srf.Length; i++)
            {
                srf[i].OnDisable();
                srf[i].OnEnable();
            }
        }
    }
}