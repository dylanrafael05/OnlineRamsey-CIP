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
            Internal = from;

            // Debug.Log(from);

            Start = graph.NodeFromID(from.Start);
            End = graph.NodeFromID(from.End);

            Length = from.Length;

            IEnumerable<List<Node>> FindSequence(List<Node> nodesSoFar, HashSet<Node> nodesRemaining)
            {
                if(nodesRemaining.Count == 0)
                {
                    if(nodesSoFar.Last().ID == from.End)
                    {
                        yield return nodesSoFar.ToList();
                    }

                    yield break;
                }

                foreach(var node in nodesRemaining.ToList())
                {
                    var edgeToNext = nodesSoFar.Last().EdgeConnectedTo(node);
                    if(edgeToNext != null && edgeToNext.Type == type)
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

            nodes = new(() => 
            {
                var allNodes = mathutils.BitPositions(Internal.Mask ^ (Bit256.One << Start.ID)).Select(graph.NodeFromID).ToHashSet();
                return FindSequence(new() {Start}, allNodes).First();
            });

            Type = type;
        }

        internal JobPathInternal Internal { get; }
        private readonly Lazy<List<Node>> nodes;

        public int Type { get; }
        public IEnumerable<Node> Nodes => nodes.Value;

        public Node Start { get; }
        public Node End { get; }
        public int Length { get; }

        public IEnumerable<Edge> Edges
            => PathUtils.GetEdgesConnecting(nodes.Value);

        public bool Contains(Node node)
            => (Internal.Mask & (Bit256.One << node.ID)) != 0;

        public bool IsEndpoint(Node node)
            => node == Start || node == End;
    }
}