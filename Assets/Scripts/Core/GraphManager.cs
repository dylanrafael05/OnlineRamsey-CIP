using System.Collections.Generic;

namespace Ramsey.Core
{
    public class GraphManager
    {
        private long lastNodeID = 0;

        private List<Node> nodes = new List<Node>();
        private List<Edge> edges = new List<Edge>();

        public IReadOnlyList<Node> Nodes => nodes;
        public IReadOnlyList<Edge> Edges => edges;

        public Edge CreateEdge(Node start, Node end, int type)
        {
            var edge = new Edge(start, end, type);
            edges.Add(edge);

            return edge;
        }

        public Node CreateNode() 
        {
            var node = new Node(lastNodeID + 1);
            nodes.Add(node);

            lastNodeID++;

            return node;
        }
    }
}