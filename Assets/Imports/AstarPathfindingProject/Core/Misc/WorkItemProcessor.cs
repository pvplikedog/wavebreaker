using System;
using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Pathfinding
{
	/// <summary>
	///     An item of work that can be executed when graphs are safe to update.
	///     See: <see cref="AstarPath.UpdateGraphs" />
	///     See: <see cref="AstarPath.AddWorkItem" />
	/// </summary>
	public struct AstarWorkItem
    {
	    /// <summary>
	    ///     Init function.
	    ///     May be null if no initialization is needed.
	    ///     Will be called once, right before the first call to <see cref="update" />.
	    /// </summary>
	    public Action init;

	    /// <summary>
	    ///     Init function.
	    ///     May be null if no initialization is needed.
	    ///     Will be called once, right before the first call to <see cref="update" />.
	    ///     A context object is sent as a parameter. This can be used
	    ///     to for example queue a flood fill that will be executed either
	    ///     when a work item calls EnsureValidFloodFill or all work items have
	    ///     been completed. If multiple work items are updating nodes
	    ///     so that they need a flood fill afterwards, using the QueueFloodFill
	    ///     method is preferred since then only a single flood fill needs
	    ///     to be performed for all of the work items instead of one
	    ///     per work item.
	    /// </summary>
	    public Action<IWorkItemContext> initWithContext;

	    /// <summary>
	    ///     Update function, called once per frame when the work item executes.
	    ///     Takes a param force. If that is true, the work item should try to complete the whole item in one go instead
	    ///     of spreading it out over multiple frames.
	    ///     Returns: True when the work item is completed.
	    /// </summary>
	    public Func<bool, bool> update;

	    /// <summary>
	    ///     Update function, called once per frame when the work item executes.
	    ///     Takes a param force. If that is true, the work item should try to complete the whole item in one go instead
	    ///     of spreading it out over multiple frames.
	    ///     Returns: True when the work item is completed.
	    ///     A context object is sent as a parameter. This can be used
	    ///     to for example queue a flood fill that will be executed either
	    ///     when a work item calls EnsureValidFloodFill or all work items have
	    ///     been completed. If multiple work items are updating nodes
	    ///     so that they need a flood fill afterwards, using the QueueFloodFill
	    ///     method is preferred since then only a single flood fill needs
	    ///     to be performed for all of the work items instead of one
	    ///     per work item.
	    /// </summary>
	    public Func<IWorkItemContext, bool, bool> updateWithContext;

        public AstarWorkItem(Func<bool, bool> update)
        {
            init = null;
            initWithContext = null;
            updateWithContext = null;
            this.update = update;
        }

        public AstarWorkItem(Func<IWorkItemContext, bool, bool> update)
        {
            init = null;
            initWithContext = null;
            updateWithContext = update;
            this.update = null;
        }

        public AstarWorkItem(Action init, Func<bool, bool> update = null)
        {
            this.init = init;
            initWithContext = null;
            this.update = update;
            updateWithContext = null;
        }

        public AstarWorkItem(Action<IWorkItemContext> init, Func<IWorkItemContext, bool, bool> update = null)
        {
            this.init = null;
            initWithContext = init;
            this.update = null;
            updateWithContext = update;
        }
    }

    /// <summary>Interface to expose a subset of the WorkItemProcessor functionality</summary>
    public interface IWorkItemContext
    {
	    /// <summary>
	    ///     Call during work items to queue a flood fill.
	    ///     An instant flood fill can be done via FloodFill()
	    ///     but this method can be used to batch several updates into one
	    ///     to increase performance.
	    ///     WorkItems which require a valid Flood Fill in their execution can call EnsureValidFloodFill
	    ///     to ensure that a flood fill is done if any earlier work items queued one.
	    ///     Once a flood fill is queued it will be done after all WorkItems have been executed.
	    ///     Deprecated: Avoid using. This will force a full recalculation of the connected components. In most cases the
	    ///     HierarchicalGraph class takes care of things automatically behind the scenes now. In pretty much all cases you
	    ///     should be able to remove the call to this function.
	    /// </summary>
	    [Obsolete(
            "Avoid using. This will force a full recalculation of the connected components. In most cases the HierarchicalGraph class takes care of things automatically behind the scenes now. In pretty much all cases you should be able to remove the call to this function.")]
        void QueueFloodFill();

	    /// <summary>
	    ///     If a WorkItem needs to have a valid area information during execution, call this method to ensure there are no
	    ///     pending flood fills.
	    ///     If you are using the <see cref="Pathfinding.GraphNode.Area" /> property or the
	    ///     <see cref="Pathfinding.PathUtilities.IsPathPossible" /> method in your work items, then you might want to call this
	    ///     method before you use them
	    ///     to ensure that the data is up to date.
	    ///     See: <see cref="Pathfinding.HierarchicalGraph" />
	    ///     <code>
	    /// AstarPath.active.AddWorkItem(new AstarWorkItem((IWorkItemContext ctx) => {
	    ///     ctx.EnsureValidFloodFill();
	    /// 
	    ///     // The above call guarantees that this method has up to date information about the graph
	    ///     if (PathUtilities.IsPathPossible(someNode, someOtherNode)) {
	    ///         // Do something
	    ///     }
	    /// }));
	    /// </code>
	    /// </summary>
	    void EnsureValidFloodFill();

	    /// <summary>
	    ///     Trigger a graph modification event.
	    ///     This will cause a <see cref="Pathfinding.GraphModifier.PostUpdate" /> event to be issued after all graph updates
	    ///     have finished.
	    ///     Some scripts listen for this event. For example off-mesh links listen to it and will recalculate which nodes they
	    ///     are connected to when it it sent.
	    ///     If a graph is dirtied multiple times, or even if multiple graphs are dirtied, the event will only be sent once.
	    /// </summary>
	    void SetGraphDirty(NavGraph graph);
    }

    internal class WorkItemProcessor : IWorkItemContext
    {
        private readonly AstarPath astar;
        private readonly IndexedQueue<AstarWorkItem> workItems = new();

        private bool anyGraphsDirty = true;

        /// <summary>
        ///     True if any work items have queued a flood fill.
        ///     See: QueueWorkItemFloodFill
        /// </summary>
        private bool queuedWorkItemFloodFill;

        public WorkItemProcessor(AstarPath astar)
        {
            this.astar = astar;
        }

        /// <summary>
        ///     Used to prevent waiting for work items to complete inside other work items as that will cause the program to
        ///     hang
        /// </summary>
        public bool workItemsInProgressRightNow { get; private set; }

        /// <summary>True if any work items are queued right now</summary>
        public bool anyQueued => workItems.Count > 0;

        /// <summary>
        ///     True while a batch of work items are being processed.
        ///     Set to true when a work item is started to be processed, reset to false when all work items are complete.
        ///     Work item updates are often spread out over several frames, this flag will be true during the whole time the
        ///     updates are in progress.
        /// </summary>
        public bool workItemsInProgress { get; private set; }

        /// <summary>
        ///     Call during work items to queue a flood fill.
        ///     An instant flood fill can be done via FloodFill()
        ///     but this method can be used to batch several updates into one
        ///     to increase performance.
        ///     WorkItems which require a valid Flood Fill in their execution can call EnsureValidFloodFill
        ///     to ensure that a flood fill is done if any earlier work items queued one.
        ///     Once a flood fill is queued it will be done after all WorkItems have been executed.
        /// </summary>
        void IWorkItemContext.QueueFloodFill()
        {
            queuedWorkItemFloodFill = true;
        }

        void IWorkItemContext.SetGraphDirty(NavGraph graph)
        {
            anyGraphsDirty = true;
        }

        /// <summary>
        ///     If a WorkItem needs to have a valid area information during execution, call this method to ensure there are no
        ///     pending flood fills
        /// </summary>
        public void EnsureValidFloodFill()
        {
            if (queuedWorkItemFloodFill)
                astar.hierarchicalGraph.RecalculateAll();
            else
                astar.hierarchicalGraph.RecalculateIfNecessary();
        }

        public void OnFloodFill()
        {
            queuedWorkItemFloodFill = false;
        }

        /// <summary>
        ///     Add a work item to be processed when pathfinding is paused.
        ///     See: ProcessWorkItems
        /// </summary>
        public void AddWorkItem(AstarWorkItem item)
        {
            workItems.Enqueue(item);
        }

        /// <summary>
        ///     Process graph updating work items.
        ///     Process all queued work items, e.g graph updates and the likes.
        ///     Returns:
        ///     - false if there are still items to be processed.
        ///     - true if the last work items was processed and pathfinding threads are ready to be resumed.
        ///     See: AddWorkItem
        ///     See: threadSafeUpdateState
        ///     See: Update
        /// </summary>
        public bool ProcessWorkItems(bool force)
        {
            if (workItemsInProgressRightNow)
                throw new Exception(
                    "Processing work items recursively. Please do not wait for other work items to be completed inside work items. " +
                    "If you think this is not caused by any of your scripts, this might be a bug.");

            Physics2D.SyncTransforms();
            workItemsInProgressRightNow = true;
            astar.data.LockGraphStructure(true);
            while (workItems.Count > 0)
            {
                // Working on a new batch
                if (!workItemsInProgress)
                {
                    workItemsInProgress = true;
                    queuedWorkItemFloodFill = false;
                }

                // Peek at first item in the queue
                var itm = workItems[0];
                bool status;

                try
                {
                    // Call init the first time the item is seen
                    if (itm.init != null)
                    {
                        itm.init();
                        itm.init = null;
                    }

                    if (itm.initWithContext != null)
                    {
                        itm.initWithContext(this);
                        itm.initWithContext = null;
                    }

                    // Make sure the item in the queue is up to date
                    workItems[0] = itm;

                    if (itm.update != null)
                        status = itm.update(force);
                    else if (itm.updateWithContext != null)
                        status = itm.updateWithContext(this, force);
                    else
                        status = true;
                }
                catch
                {
                    workItems.Dequeue();
                    workItemsInProgressRightNow = false;
                    astar.data.UnlockGraphStructure();
                    throw;
                }

                if (!status)
                {
                    if (force)
                        Debug.LogError(
                            "Misbehaving WorkItem. 'force'=true but the work item did not complete.\nIf force=true is passed to a WorkItem it should always return true.");

                    // Still work items to process
                    workItemsInProgressRightNow = false;
                    astar.data.UnlockGraphStructure();
                    return false;
                }

                workItems.Dequeue();
            }

            EnsureValidFloodFill();

            Profiler.BeginSample("PostUpdate");
            if (anyGraphsDirty) GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);
            Profiler.EndSample();

            anyGraphsDirty = false;
            workItemsInProgressRightNow = false;
            workItemsInProgress = false;
            astar.data.UnlockGraphStructure();
            return true;
        }

        /// <summary>Similar to Queue<T> but allows random access</summary>
        private class IndexedQueue<T>
        {
            private T[] buffer = new T[4];
            private int start;

            public T this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
                    return buffer[(start + index) % buffer.Length];
                }
                set
                {
                    if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
                    buffer[(start + index) % buffer.Length] = value;
                }
            }

            public int Count { get; private set; }

            public void Enqueue(T item)
            {
                if (Count == buffer.Length)
                {
                    var newBuffer = new T[buffer.Length * 2];
                    for (var i = 0; i < Count; i++) newBuffer[i] = this[i];
                    buffer = newBuffer;
                    start = 0;
                }

                buffer[(start + Count) % buffer.Length] = item;
                Count++;
            }

            public T Dequeue()
            {
                if (Count == 0) throw new InvalidOperationException();
                var item = buffer[start];
                start = (start + 1) % buffer.Length;
                Count--;
                return item;
            }
        }
    }
}