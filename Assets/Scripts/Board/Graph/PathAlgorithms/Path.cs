using System;
using System.Collections.Generic;
using System.Linq;
using Ramsey.Utilities;
using UnityEngine;

namespace Ramsey.Graph
{
    public interface IPath
    {
        int Type { get; }
        IEnumerable<Node> Nodes { get; }

        int Length { get; }
        Node Start { get; }
        Node End { get; }

        IEnumerable<Edge> Edges { get; }

        bool Contains(Node node);
        bool IsEndpoint(Node node);
    }

    internal static class PathUtils
    {
        public static IEnumerable<Edge> GetEdgesConnecting(IEnumerable<Node> nodes) 
        {
            Node last = null;
            foreach (var node in nodes.ToList())
            {
                if (last != null)
                {
                    yield return node.EdgeConnectedTo(last)
                        ?? throw new InvalidOperationException("Edges must connect the nodes in a path.");
                }

                last = node;
            }
        }
    }

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

        public Node Start => nodes.First();
        public Node End => nodes.Last();

        public bool Contains(Node node)
        {
            return nodeSet.IsSet(node.ID);
        }

        public Path Append(Node node)
        {
            var newNodes = nodes.Append(node);
            var newSet = nodeSet.Clone();
            newSet.Set(node.ID);

            return new Path(newNodes, newSet, Type);
        }
        public Path Prepend(Node node)
        {
            var newNodes = nodes.Prepend(node);
            var newSet = nodeSet.Clone();
            newSet.Set(node.ID);

            return new Path(newNodes, newSet, Type);
        }

        public bool IsEndpoint(Node node)
            => node == Start || node == End;

        public Path Expand(Node node, Node from)
        {
            if (from == Start) return Prepend(node);
            else if (from == End) return Append(node);

            throw new InvalidOperationException("Cannot expand a path from a non-endpoint!");
        }
        public bool IsIdenticalTo(Path other)
        {
            if (nodeSet.SetEquals(other.nodeSet))
            {
                if (Start == other.Start)
                {
                    return End == other.End
                        && nodes.SequenceEqual(other.nodes);
                }
                else if (Start == other.End)
                {
                    return End == other.Start
                        && nodes.SequenceEqual(other.nodes.Reverse());
                }
            }

            return false;
        }

        private IEnumerable<Edge> GetEdges()
            => PathUtils.GetEdgesConnecting(Nodes);

        public IEnumerable<Edge> Edges
            => GetEdges();

        public override string ToString()
            => $"{{Length = {Length}, Type = {Type}, Nodes = [{string.Join(", ", Nodes.Select(n => n.ID))}]}}";
    }
}