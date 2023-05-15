using System.Collections.Generic;

namespace Ramsey.Core
{
    public class GraphManager
    {
        public IReadOnlyList<Node> Nodes { get; }
        public IReadOnlyList<Edge> Edges { get; }

        public void AddEdge(Edge edge)
        {}

        public void AddNode(Node node) 
        {}
    }
}