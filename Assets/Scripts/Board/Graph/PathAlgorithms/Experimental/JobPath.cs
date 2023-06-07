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
    public class JobPath : IPath
    {
        private static readonly ProfilerMarker GetNodesMarker = new(ProfilerCategory.Scripts, "JobPath.GetNodes");

        internal JobPath(JobPathInternal from, int type, Graph graph)
        {
            Internal = from;

            Start = graph?.NodeFromID(from.Start);
            End = graph?.NodeFromID(from.End);

            Length = from.Length;
            Type = type;
            
            using(GetNodesMarker.Auto())
            {
                var allNodes = MathUtils.BitPositions(Internal.Mask ^ (Bit256.One << Start.ID)).Select(graph.NodeFromID).ToHashSet();

                var reconstructedMask = Bit256.Zero;
                foreach(var n in allNodes)
                {
                    reconstructedMask |= Bit256.One << n.ID;
                }

                Assert.AreEqual(Internal.Mask ^ (Bit256.One << Start.ID), reconstructedMask, "They must be equal!");

                nodes = FindSequence(new() {Start}, allNodes).First();
            }
        }

        internal JobPathInternal Internal { get; }
        private List<Node> nodes;

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

        public IEnumerable<Node> Nodes => nodes;

        public Node Start { get; }
        public Node End { get; }
        public int Length { get; }
        public int Type { get; }

        public IEnumerable<Edge> Edges
            => PathUtils.GetEdgesConnecting(nodes);

        public bool Contains(Node node)
            => (Internal.Mask & (Bit256.One << node.ID)) != 0;

        public bool IsEndpoint(Node node)
            => node == Start || node == End;
    }
}