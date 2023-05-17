using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Ramsey.Core
{
    public class Graph : IGraph
    {
        private readonly List<Node> nodes = new();
        private readonly List<Edge> edges = new();

        public IReadOnlyList<Node> Nodes => nodes;
        public IReadOnlyList<Edge> Edges => edges;

        public bool IsValidEdge(Node start, Node end) 
        {
            if(start == end) return false;
            if(start.IsConnectedTo(end)) return false;

            return true;
        }

        public Edge CreateEdge(Node start, Node end, int type = -1)
        {
            var edge = new Edge(start, end, type, edges.Count);
            edges.Add(edge);

            start.RegisterToEdge(edge);
            end.RegisterToEdge(edge);

            return edge;
        }

        public Node CreateNode(float2 position = default) 
        {
            var node = new Node(nodes.Count) {Position = position};
            nodes.Add(node);

            return node;
        }

        public void MoveNode(Node node, float2 position)
        {
            node.Position = position;
        }
        public void PaintEdge(Edge edge, int type)
        {
            Assert.AreEqual(edge.Type, -1, "Cannot change the color of a painted edge!");

            edge.Type = type;
        }
        
        public void Clear()
        {
            nodes.Clear();
            edges.Clear();
        }

        internal bool Empty => nodes.Count == 0 && edges.Count == 0;
    }
}