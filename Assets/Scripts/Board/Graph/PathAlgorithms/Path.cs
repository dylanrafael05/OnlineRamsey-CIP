using System;
using System.Collections.Generic;
using System.Linq;
using Ramsey.Utilities;

namespace Ramsey.Graph
{
    public class Path : IPath
    {
        internal Path(VennList<Node> nodes, BitSet nodeSet, int type)
        {
            this.nodes = nodes;
            this.nodeSet = nodeSet;
            Type = type;
        }

        public Path(Node node, int type)
        {
            nodes = new(node);
            nodeSet = new();
            nodeSet.Set(node.ID);

            Type = type;
        }

        internal readonly BitSet nodeSet;
        internal readonly VennList<Node> nodes;

        public int Type { get; }

        public IEnumerable<Node> Nodes => nodes;
        public int Length => nodes.Count - 1;

        public Node End => nodes.Last();

        public bool Contains(Node node)
        {
            return nodeSet.IsSet(node.ID);
        }

        public IPath Append(Node node)
        {
            var newNodes = nodes.Append(node);
            var newSet = nodeSet.Clone();
            newSet.Set(node.ID);

            return new Path(newNodes, newSet, Type);
        }
        public IPath Prepend(Node node)
        {
            var newNodes = nodes.Prepend(node);
            var newSet = nodeSet.Clone();
            newSet.Set(node.ID);

            return new Path(newNodes, newSet, Type);
        }

        public IEnumerable<Edge> Edges
        {
            get 
            {
                var last = nodes.First();
                foreach(var next in nodes.Skip(1)) 
                {
                    yield return last.EdgeConnectedTo(next) 
                        ?? throw new InvalidOperationException("Edges must connect the nodes in a path.");
                }
            }
        }

        public override string ToString()
            => $"{{Length = {Length}, Type = {Type}, Nodes = [{string.Join(", ", Nodes.Select(n => n.ID))}]}}";
    }
}