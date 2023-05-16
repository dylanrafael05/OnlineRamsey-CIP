using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Mathematics;

namespace Ramsey.Core
{
    public class GraphManager : IGraphView
    {
        private readonly List<Node> nodes = new();
        private readonly List<Edge> edges = new();

        public IReadOnlyList<Node> Nodes => nodes;
        public IReadOnlyList<Edge> Edges => edges;

        public Edge CreateEdge(Node start, Node end, int type)
        {
            var edge = new Edge(start, end, type);
            edges.Add(edge);

            start.AddEdge(edge);
            end.AddEdge(edge);

            return edge;
        }

        public Node CreateNode() 
        {
            var node = new Node(nodes.Count);
            nodes.Add(node);

            return node;
        }

        public void SetNodePosition(Node node, float2 position)
        {
            node.Position = position;
        }

        internal void AddExistingNode(Node node) 
        {
            nodes.Add(node);
        }

        internal bool Empty => nodes.Count == 0 && edges.Count == 0;
        internal void Clear()
        {
            nodes.Clear();
            edges.Clear();
        }
    }
}