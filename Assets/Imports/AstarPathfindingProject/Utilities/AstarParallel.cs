#if UNITY_WEBGL && !UNITY_EDITOR
#define SINGLE_THREAD
#endif
using System;
using System.Collections.Generic;
using System.Threading;

namespace Pathfinding.Util
{
	/// <summary>
	///     Helper for parallelizing tasks.
	///     More specifically this class is useful if the tasks need some large and slow to initialize 'scratch pad'.
	///     Using this class you can initialize a scratch pad per thread and then use the appropriate one in the task
	///     callback (which includes a thread index).
	///     Any exception that is thrown in the worker threads will be propagated out to the caller of the <see cref="Run" />
	///     method.
	/// </summary>
	public class ParallelWorkQueue<T>
    {
        private readonly int initialCount;

        /// <summary>Queue of items</summary>
        private readonly Queue<T> queue;

        /// <summary>Number of threads to use</summary>
        public readonly int threadCount;

        /// <summary>
        ///     Callback to run for each item in the queue.
        ///     The callback takes the item as the first parameter and the thread index as the second parameter.
        /// </summary>
        public Action<T, int> action;

        public ParallelWorkQueue(Queue<T> queue)
        {
            this.queue = queue;
            initialCount = queue.Count;
#if SINGLE_THREAD
			threadCount = 1;
#else
            threadCount = Math.Min(initialCount,
                Math.Max(1, AstarPath.CalculateThreadCount(ThreadCount.AutomaticHighLoad)));
#endif
        }

        /// <summary>Execute the tasks.</summary>
        /// <param name="progressTimeoutMillis">
        ///     This iterator will yield approximately every progressTimeoutMillis milliseconds.
        ///     This can be used to e.g show a progress bar.
        /// </param>
        public IEnumerable<int> Run(int progressTimeoutMillis)
        {
            if (initialCount != queue.Count)
                throw new InvalidOperationException("Queue has been modified since the constructor");

            // Return early if there are no items in the queue.
            // This is important because WaitHandle.WaitAll with an array of length zero
            // results in weird behaviour (Microsoft's .Net returns false, Mono returns true
            // and the documentation says it should throw an exception).
            if (initialCount == 0) yield break;

#if SINGLE_THREAD
			// WebGL does not support multithreading so we will do everything on the main thread instead
			for (int i = 0; i < initialCount; i++) {
				action(queue.Dequeue(), 0);
				yield return i + 1;
			}
#else
            // Fire up a bunch of threads to scan the graph in parallel
            waitEvents = new ManualResetEvent[threadCount];
            for (var i = 0; i < waitEvents.Length; i++)
            {
                waitEvents[i] = new ManualResetEvent(false);
#if NETFX_CORE
				// Need to make a copy here, otherwise it may refer to some other index when the task actually runs.
				int threadIndex = i;
				System.Threading.Tasks.Task.Run(() => RunTask(threadIndex));
#else
                ThreadPool.QueueUserWorkItem(threadIndex => RunTask((int)threadIndex), i);
#endif
            }

            while (!WaitHandle.WaitAll(waitEvents, progressTimeoutMillis))
            {
                int count;
                lock (queue)
                {
                    count = queue.Count;
                }

                yield return initialCount - count;
            }

            if (innerException != null) throw innerException;
#endif
        }

#if !SINGLE_THREAD
        private void RunTask(int threadIndex)
        {
            try
            {
                while (true)
                {
                    T tile;
                    lock (queue)
                    {
                        if (queue.Count == 0) return;
                        tile = queue.Dequeue();
                    }

                    action(tile, threadIndex);
                }
            }
            catch (Exception e)
            {
                innerException = e;
                // Stop the remaining threads
                lock (queue)
                {
                    queue.Clear();
                }
            }
            finally
            {
                waitEvents[threadIndex].Set();
            }
        }
#endif
#if !SINGLE_THREAD
        private ManualResetEvent[] waitEvents;
        private Exception innerException;
#endif
    }
}