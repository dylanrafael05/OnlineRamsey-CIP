using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ramsey.Utilities
{
    // NOTE: should an 'index' property be stored in each node
    // to make it faster to index a venn list?

    /// <summary>
    /// An immutable list of items which shares a single allocation for 
    /// its elements with extended versions of itself created by 'appending'
    /// or 'prepending' elements.
    /// 
    /// Once an element is added to a VennList, it cannot be modified or removed.
    /// 
    /// Access to the first and last elements of a list is an O(1) operation,
    /// while any other elements must be accessed through an iterator with O(n)
    /// complexity at worst. Appending and prepending are both O(1) operations.
    /// Iterating through a venn list using <see cref="Values"/> is faster
    /// than iterating through it normally.
    /// </summary>
    public class VennList<T> : IEnumerable<T>
    {
        /// <summary>
        /// Represents an empty node.
        /// </summary>
        public static Node Empty => new EmptyNode();

        /// <summary>
        /// The structure which holds one item of the overall venn list,
        /// as well as the way that this item relates to the rest of the 
        /// list, exposed through <see cref="Next"/>.
        /// 
        /// This structure also keeps track of the first and last items
        /// of the list, as well as its count, to improve the speed of
        /// getting these values.
        /// </summary>
        public abstract class Node 
        {
            public abstract int Count { get; }
            
            public abstract T Value { get; }
            public abstract T First { get; }
            public abstract T Last { get; }

            public abstract Node Next { get; }

            public abstract bool IsEmpty { get; }
        }

        private sealed class EmptyNode : Node
        {
            public override int Count => 0;
            public override bool IsEmpty => true;

            public override T Value => throw new InvalidOperationException("Cannot get the value of an empty venn node!");
            public override T First => throw new InvalidOperationException("Cannot get the first value from an empty venn node!");
            public override T Last => throw new InvalidOperationException("Cannot get the last value from an empty venn node!");

            public override Node Next => null;
        }

        private sealed class BeforeNode : Node 
        {
            public BeforeNode(T value, Node after)
            {
                Next = after;

                Count = after.Count + 1;

                Value = value;

                First = value;
                Last = after.IsEmpty ? value : after.Last;
            }

            public override Node Next { get; }

            public override T Value { get; }
            public override T First { get; }
            public override T Last { get; }
            public override int Count { get; }

            public override bool IsEmpty => false;
        }

        private sealed class AfterNode : Node 
        {
            public AfterNode(Node before, T value)
            {
                Next = before;

                Count = before.Count + 1;

                Value = value;

                Last = value;
                First = before.IsEmpty ? value : before.First;
            }

            public override Node Next { get; }
            
            public override T Value { get; }
            public override T First { get; }
            public override T Last { get; }
            public override int Count { get; }

            public override bool IsEmpty => false;
        }

        /// <summary>
        /// Create an empty vennlist. Does not allocate a new node.
        /// </summary>
        public VennList()
        {
            node = Empty;
        }

        /// <summary>
        /// Create a vennlist which stores the given collection,
        /// without allocating the empty node at the list's core.
        /// </summary>
        public VennList(IEnumerable<T> enumerable)
        {
            node = Empty;

            foreach(var item in enumerable)
            {
                node = new AfterNode(node, item);
            }
        }
        /// <inheritdoc cref="VennList(IEnumerable{T})"/>
        public VennList(params T[] items) : this((IEnumerable<T>)items) 
        {}

        private VennList(Node n)
        {
            node = n;
        }

        private readonly Node node;

        /// <summary>
        /// Get the amount of items in this vennlist.
        /// </summary>
        public int Count => node.Count;

        /// <summary>
        /// Create a new vennlist which appends the given value
        /// to this vennlist.
        /// </summary>
        public VennList<T> Append(T item)
        {
            return new(new AfterNode(node, item));
        }
        /// <summary>
        /// Create a new vennlist which prepends the given value
        /// to this vennlist.
        /// </summary>
        public VennList<T> Prepend(T item) 
        {
            return new(new BeforeNode(item, node));
        }

        /// <summary>
        /// Checks if a given value exists within this list.
        /// </summary>
        public bool Contains(T item) 
        {
            return Values.Contains(item);
        }

        /// <summary>
        /// Get the first element of this list.
        /// </summary>
        public T First() => node.First;
        /// <summary>
        /// Get the last element of this list.
        /// </summary>
        public T Last() => node.Last;

        private IEnumerable<T> EnumerateNode(Node n)
        {
            if(n.IsEmpty)
            {
                yield break;
            }
            else if(n is BeforeNode)
            {
                yield return n.Value;
                foreach(var next in EnumerateNode(n.Next))
                {
                    yield return next;
                }
            }
            else 
            {
                foreach(var next in EnumerateNode(n.Next))
                {
                    yield return next;
                }
                yield return n.Value;
            }
        }

        /// <summary>
        /// Returns an enumerator which lists all values contained by this list
        /// without being in order. Iterating this is faster than iterating
        /// the values in order.
        /// </summary>
        public IEnumerable<T> Values
        {
            get 
            {
                var node = this.node;
                while(!node.IsEmpty)
                {
                    yield return node.Value;
                    node = node.Next;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
            => EnumerateNode(node).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}