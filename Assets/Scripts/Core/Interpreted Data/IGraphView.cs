using System.Collections.Generic;

namespace Ramsey.Core
{
    public interface IGraphView
    {
        IReadOnlyList<Node> Nodes {get;}
        IReadOnlyList<Edge> Edges {get;}

        bool IsValidEdge(Node start, Node end);
    }
}