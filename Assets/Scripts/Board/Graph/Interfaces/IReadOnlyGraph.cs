using System.Collections.Generic;

namespace Ramsey.Graph
{
    public interface IReadOnlyGraph
    {
        IReadOnlyList<Node> Nodes {get;}
        IReadOnlyList<Edge> Edges {get;}

        bool IsValidEdge(Node start, Node end);

        Node NodeFromID(int id);
        Edge EdgeFromID(int id);
    }
}