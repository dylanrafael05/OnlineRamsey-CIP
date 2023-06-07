using Unity.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Ramsey.Utilities;
using Unity.Profiling;

namespace Ramsey.Graph.Experimental
{
    public struct CellBlock<T>
    {
        private T[] values;

        public CellBlock(int capacity)
        {
            values = new T[capacity];
        }

        public Cell<T> GetCell(int index) 
        {
            return new Cell<T>(values, index);
        }
    }

    public struct Cell<T>
    {
        internal Cell(T[] values, int index) 
        {
            this.values = values;
            this.index = index;

            HasValue = false;
        }

        public bool HasValue { get; private set; }

        private readonly T[] values;
        private readonly int index;
        public T Value
        {
            get => values[index];
            set {values[index] = value; HasValue = true;}
        }
    }
    public struct JobPath : IPath
    {
        private static readonly ProfilerMarker GetNodesMarker = new(ProfilerCategory.Scripts, "JobPath.GetNodes");

        internal JobPath(JobPathInternal from, Cell<List<Node>> preallocatedNodes, Graph graph, int type)
        {
            Internal = from;

            Start = graph.NodeFromID(from.Start);
            End = graph.NodeFromID(from.End);

            Length = from.Length;
            Type = type;

            nodes = preallocatedNodes;

            this.graph = graph;
        }

        internal readonly JobPathInternal Internal { get; }
        private readonly Graph graph;

        private Cell<List<Node>> nodes;

        private IEnumerable<List<Node>> FindSequence(List<Node> nodesSoFar, HashSet<Node> nodesRemaining)
        {
            if(nodesRemaining.Count == 0)
            {
                if(nodesSoFar.Last().ID == Internal.End)
                {
                    yield return nodesSoFar.ToList();
                }

                yield break;
            }

            foreach(var node in nodesRemaining.ToList())
            {
                var edgeToNext = nodesSoFar.Last().EdgeConnectedTo(node);
                if(edgeToNext != null && edgeToNext.Type == Type)
                {
                    nodesRemaining.Remove(node);
                    nodesSoFar.Add(node);

                    foreach(var seq in FindSequence(nodesSoFar, nodesRemaining))
                    {
                        yield return seq;
                    }

                    nodesRemaining.Add(node);
                    nodesSoFar.RemoveAt(nodesSoFar.Count - 1);
                }
            }
        }

        private List<Node> GetNodes()
        {
            using(GetNodesMarker.Auto())
            {
                if(!nodes.HasValue)
                {
                    var allNodes = MathUtils.BitPositions(Internal.Mask ^ (Bit256.One << Start.ID)).Select(graph.NodeFromID).ToHashSet();
                    nodes.Value = FindSequence(new() {Start}, allNodes).First();
                }

                return nodes.Value;
            }
        }

        public int Type { get; }
        public IEnumerable<Node> Nodes => GetNodes();

        public Node Start { get; }
        public Node End { get; }
        public int Length { get; }

        public IEnumerable<Edge> Edges
            => PathUtils.GetEdgesConnecting(GetNodes());

        public bool Contains(Node node)
            => (Internal.Mask & (Bit256.One << node.ID)) != 0;

        public bool IsEndpoint(Node node)
            => node == Start || node == End;
    }
}