using System;
using System.Collections.Generic;

namespace Pathfinding.Util
{
	/// <summary>
	///     Holds a lookup datastructure to quickly find objects inside rectangles.
	///     Objects of type T occupy an integer rectangle in the grid and they can be
	///     moved efficiently. You can query for all objects that touch a specified
	///     rectangle that runs in O(m*k+r) time where m is the number of objects that
	///     the query returns, k is the average number of cells that an object
	///     occupies and r is the area of the rectangle query.
	///     All objects must be contained within a rectangle with one point at the origin
	///     (inclusive) and one at <see cref="size" /> (exclusive) that is specified in the constructor.
	/// </summary>
	public class GridLookup<T> where T : class
    {
	    /// <summary>
	    ///     Linked list of all items.
	    ///     Note that the first item in the list is a dummy item and does not contain any data.
	    /// </summary>
	    private readonly Root all = new();

        private readonly Item[] cells;
        private readonly Stack<Item> itemPool = new();
        private readonly Dictionary<T, Root> rootLookup = new();
        private readonly Int2 size;

        public GridLookup(Int2 size)
        {
            this.size = size;
            cells = new Item[size.x * size.y];
            for (var i = 0; i < cells.Length; i++) cells[i] = new Item();
        }

        /// <summary>Linked list of all items</summary>
        public Root AllItems => all.next;

        public void Clear()
        {
            rootLookup.Clear();
            all.next = null;
            foreach (var item in cells) item.next = null;
        }

        public Root GetRoot(T item)
        {
            Root root;

            rootLookup.TryGetValue(item, out root);
            return root;
        }

        /// <summary>
        ///     Add an object to the lookup data structure.
        ///     Returns: A handle which can be used for Move operations
        /// </summary>
        public Root Add(T item, IntRect bounds)
        {
            var root = new Root
            {
                obj = item,
                prev = all,
                next = all.next
            };

            all.next = root;
            if (root.next != null) root.next.prev = root;

            rootLookup.Add(item, root);
            Move(item, bounds);
            return root;
        }

        /// <summary>Removes an item from the lookup data structure</summary>
        public void Remove(T item)
        {
            Root root;

            if (!rootLookup.TryGetValue(item, out root)) return;

            // Make the item occupy no cells at all
            Move(item, new IntRect(0, 0, -1, -1));
            rootLookup.Remove(item);
            root.prev.next = root.next;
            if (root.next != null) root.next.prev = root.prev;
        }

        /// <summary>Move an object to occupy a new set of cells</summary>
        public void Move(T item, IntRect bounds)
        {
            Root root;

            if (!rootLookup.TryGetValue(item, out root))
                throw new ArgumentException("The item has not been added to this object");

            var prev = root.previousBounds;
            if (prev == bounds) return;

            // Remove all
            for (var i = 0; i < root.items.Count; i++)
            {
                var ob = root.items[i];
                ob.prev.next = ob.next;
                if (ob.next != null) ob.next.prev = ob.prev;
            }

            root.previousBounds = bounds;
            var reusedItems = 0;
            for (var z = bounds.ymin; z <= bounds.ymax; z++)
            for (var x = bounds.xmin; x <= bounds.xmax; x++)
            {
                Item ob;
                if (reusedItems < root.items.Count)
                {
                    ob = root.items[reusedItems];
                }
                else
                {
                    ob = itemPool.Count > 0 ? itemPool.Pop() : new Item();
                    ob.root = root;
                    root.items.Add(ob);
                }

                reusedItems++;

                ob.prev = cells[x + z * size.x];
                ob.next = ob.prev.next;
                ob.prev.next = ob;
                if (ob.next != null) ob.next.prev = ob;
            }

            for (var i = root.items.Count - 1; i >= reusedItems; i--)
            {
                var ob = root.items[i];
                ob.root = null;
                ob.next = null;
                ob.prev = null;
                root.items.RemoveAt(i);
                itemPool.Push(ob);
            }
        }

        /// <summary>
        ///     Returns all objects of a specific type inside the cells marked by the rectangle.
        ///     Note: For better memory usage, consider pooling the list using Pathfinding.Util.ListPool after you are done with it
        /// </summary>
        public List<U> QueryRect<U>(IntRect r) where U : class, T
        {
            var result = ListPool<U>.Claim();

            // Loop through tiles and check which objects are inside them
            for (var z = r.ymin; z <= r.ymax; z++)
            {
                var zs = z * size.x;
                for (var x = r.xmin; x <= r.xmax; x++)
                {
                    var c = cells[x + zs];
                    // Note, first item is a dummy, so it is ignored
                    while (c.next != null)
                    {
                        c = c.next;
                        var obj = c.root.obj as U;
                        if (!c.root.flag && obj != null)
                        {
                            c.root.flag = true;
                            result.Add(obj);
                        }
                    }
                }
            }

            // Reset flags
            for (var z = r.ymin; z <= r.ymax; z++)
            {
                var zs = z * size.x;
                for (var x = r.xmin; x <= r.xmax; x++)
                {
                    var c = cells[x + zs];
                    while (c.next != null)
                    {
                        c = c.next;
                        c.root.flag = false;
                    }
                }
            }

            return result;
        }

        internal class Item
        {
            public Item prev, next;
            public Root root;
        }

        public class Root
        {
            internal bool flag;

            /// <summary>References to an item in each grid cell that this object is contained inside</summary>
            internal List<Item> items = new();

            /// <summary>Next item in the linked list of all roots</summary>
            public Root next;

            /// <summary>Underlying object</summary>
            public T obj;

            /// <summary>Previous item in the linked list of all roots</summary>
            internal Root prev;

            internal IntRect previousBounds = new(0, 0, -1, -1);
        }
    }
}