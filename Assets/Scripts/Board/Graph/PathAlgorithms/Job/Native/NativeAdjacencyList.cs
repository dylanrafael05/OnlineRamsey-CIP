using Unity.Collections;
using System;
using UnityEngine;

namespace Ramsey.Graph.Experimental
{
    /// <summary>
    /// Stores an efficient and native adjacency list.
    /// </summary>
    public struct NativeAdjacencyList : IDisposable 
    {
        public NativeAdjacencyList(int nodecount, int maxlength, Allocator alloc) 
        {
            NodeCount = nodecount;
            Depth = maxlength + 1;

            innerArray = new(NodeCount * Depth, alloc);

            for(int i = 0; i < innerArray.Length; i++)
            {
                innerArray[i] = -1;
            }
        }

        private NativeArray<short> innerArray;
        public int NodeCount { get; }
        public int Depth { get; }

        public void Add(byte node, byte adj)
        {
            var idx = node*Depth;

            while(innerArray[idx] >= 0)
            {
                idx++;
            }

            innerArray[idx] = adj;
        }

        public struct Iterator 
        {
            internal Iterator(NativeAdjacencyList nl, int startIndex)
            {
                this.nl = nl;
                index = startIndex;
            }

            private readonly NativeAdjacencyList nl;
            private int index;

            public bool Move(out byte value)
            {
                var v = nl.innerArray[index];
                value = (byte)v;
                index++;
                return v >= 0;
            }
        }

        public readonly Iterator GetIterator(int node)
        {
            return new Iterator(this, Depth * node);
        }

        public void Dispose()
        {
            innerArray.Dispose();
        }
    }
}