using System.Collections.Generic;

namespace Ramsey.Graph
{
    public interface IReadOnlyGraph
    {
        IEnumerable<Node> Nodes {get;}
        IEnumerable<Edge> Edges {get;}

        bool IsValidEdge(Node start, Node end);

        Node NodeFromID(int id);
        Edge EdgeFromID(int id);
    }
}