using Unity.Collections;
using System.Collections.Generic;
using System.Linq;
using Ramsey.Utilities;
using Unity.Profiling;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ramsey.Graph.Experimental
{
    /// <summary>
    /// A path generated from a JobPathInternal.
    /// </summary>
    public class JobPath : IPath
    {
        private static readonly ProfilerMarker GetNodesMarker = new(ProfilerCategory.Scripts, "JobPath.GetNodes");

        internal JobPath(JobPathInternal from, int type, IReadOnlyGraph graph)
        {
            Internal = from;

            Start = graph?.NodeFromID(from.Start);
            End = graph?.NodeFromID(from.End);

            Length = from.Length;
            Type = type;
            
            using(GetNodesMarker.Auto())
            {
                var remainingNodes = Internal.Mask ^ (Bit256.One << Start.ID);

                nodes = new Node[Length + 1];
                nodes[0]  = Start;
                nodes[^1] = End;

                Assert.IsTrue(
                    FindSequence(graph, nodes, 1, remainingNodes), 
                    "Must be able to find a sequence!");
                
                Middle = nodes[(int) ((nodes.Length - 1) * .5f)];
            }
        }

        /// <summary>
        /// Find an ordering of the given nodes such that they form a contiguous path 
        /// connecting a start and end node, passed through the first and last indices of 
        /// nodesOut. Then, store this ordering inside the nodesOut parameter.
        /// </summary>
        private bool FindSequence(IReadOnlyGraph g, Node[] nodesOut, int startCount, Bit256 nodesRemaining)
        {
            if(Bit256.Bitcount(nodesRemaining) == 1)
            {
                var edgeToEnd = nodesOut[startCount-1].EdgeConnectedTo(End);

                return Bit256.FirstBitpos(nodesRemaining) == End.ID && edgeToEnd != null && edgeToEnd.Type == Type;
            }

            var iter = Bit256.IterateBitpos(nodesRemaining);

            while(iter.GetNext(out var nodeID))
            {
                var node = g.NodeFromID(nodeID);
                var edgeToNext = nodesOut[startCount-1].EdgeConnectedTo(node);

                if(edgeToNext != null && edgeToNext.Type == Type)
                {
                    nodesOut[startCount] = node;

                    if(FindSequence(g, nodesOut, startCount + 1, nodesRemaining ^ (Bit256.One << nodeID)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal JobPathInternal Internal { get; }
        private readonly Node[] nodes;

        public IEnumerable<Node> Nodes => nodes;

        public Node Start { get; }
        public Node Middle { get; }
        public Node End { get; }
        public int Length { get; }
        public int Type { get; }

        public IEnumerable<Edge> Edges
            => PathUtils.GetEdgesConnecting(nodes);
        public IEnumerable<DirectedEdge> DirectedEdges
            => PathUtils.GetDirectedEdgesConnecting(nodes);

        public bool Contains(Node node)
            => (Internal.Mask & (Bit256.One << node.ID)) != 0;

        public bool IsEndpoint(Node node)
            => node == Start || node == End;
    }
}