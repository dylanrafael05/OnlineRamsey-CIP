using Unity.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Ramsey.Utilities;

namespace Ramsey.Graph.Experimental
{
    public class JobPath : IPath
    {
        internal JobPath(JobPathInternal from, Graph graph, int type)
        {
            mask = from.Mask;

            // Debug.Log(from);

            Start = graph.NodeFromID(from.Start);
            End = graph.NodeFromID(from.End);

            Length = from.Length;

            IEnumerable<List<Node>> FindSequence(List<Node> nodesSoFar, List<Node> nodesRemaining)
            {
                if(nodesRemaining.Count == 0)
                {
                    if(nodesSoFar.Last().ID == from.End)
                    {
                        yield return nodesSoFar;
                    }

                    yield break;
                }

                foreach(var node in nodesRemaining)
                {
                    var edgeToNext = nodesSoFar.Last().EdgeConnectedTo(node);
                    if(edgeToNext != null && edgeToNext.Type == type)
                    {
                        foreach(var seq in FindSequence(nodesSoFar.Append(node).ToList(), nodesRemaining.Where(n => n != node).ToList()))
                        {
                            yield return seq;
                        }
                    }
                }
            }

            var allNodes = MathUtils.BitPositions(mask ^ (1UL << from.Start)).Select(graph.NodeFromID).ToList();
            nodes = new(() => FindSequence(new() {Start}, allNodes).First());

            Type = type;
        }

        private readonly ulong mask;
        private readonly Lazy<List<Node>> nodes;

        public int Type { get; }
        public IEnumerable<Node> Nodes => nodes.Value;

        public Node Start { get; }
        public Node End { get; }
        public int Length { get; }

        public IEnumerable<Edge> Edges
            => PathUtils.GetEdgesConnecting(nodes.Value);

        public bool Contains(Node node)
            => (mask | (1UL << node.ID)) == mask;

        public bool IsEndpoint(Node node)
            => node == Start || node == End;
    }
}