using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Ramsey.Core
{
    public class Edge 
    {
        internal Edge(Node start, Node end, int type)
        {
            Assert.AreNotEqual(start, end, "Cannot create an edge which connects a node to itself");

            Start = start;
            End = end;
            Type = type;
        }

        public Node Start { get; }
        public Node End { get; }
        public int Type { get; }

        internal Node NodeOpposite(Node node) 
        {
            if(node == Start) return End;
            if(node == End) return Start;

            throw new InvalidOperationException("Cannot find node opposite to a node not related to this edge.");
        }
    }

    public class Path 
    {
        private Path(IEnumerable<Edge> edges, IEnumerable<Node> nodes) 
        {
            this.edges = edges.ToList();
            this.nodes = nodes.ToList();
        }

        internal Path(Edge edge)
        {
            edges = new() {edge};
            nodes = new() {edge.Start, edge.End};
        }

        internal Path Append(Edge edge)
        {
            // Edge connects to start
            if(edge.Start == nodes.First())
            {
                return new Path(edges.Prepend(edge), nodes.Prepend(edge.End));
            }

            if(edge.End == nodes.First())
            {
                return new Path(edges.Prepend(edge), nodes.Prepend(edge.Start));
            }

            // Edge connects to end
            if(edge.Start == nodes.Last())
            {
                return new Path(edges.Append(edge), nodes.Append(edge.End));
            }

            if(edge.End == nodes.Last())
            {
                return new Path(edges.Append(edge), nodes.Append(edge.Start));
            }

            throw new InvalidOperationException("Cannot append an edge to a path which doesnt intersect it!");
        }

        //TODO: use better data structures here!
        private List<Edge> edges;
        private List<Node> nodes;

        internal bool IsEndpoint(Node node) 
            => node == nodes.First() || node == nodes.Last();

        internal IEnumerable<Path> SplitAgainst(Edge edge)
        {
            if(IsEndpoint(edge.Start))
            {
                yield return null;
            }
        }


    }
}