using System.Collections.Generic;

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
        IEnumerable<DirectedEdge> DirectedEdges { get; }

        bool Contains(Node node);
        bool IsEndpoint(Node node);
    }
}