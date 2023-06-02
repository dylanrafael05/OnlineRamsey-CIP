using System.Collections.Generic;
using Unity.Mathematics;

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