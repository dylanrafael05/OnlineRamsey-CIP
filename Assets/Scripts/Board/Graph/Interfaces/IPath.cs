using System.Collections.Generic;

namespace Ramsey.Graph
{
    public interface IPath
    {
        int Type { get; }
        int Length { get; }

        IEnumerable<Node> Nodes { get; }
        Node End { get; }

        IEnumerable<Edge> Edges { get; }

        IPath Append(Node node);
        IPath Prepend(Node node);

        bool Contains(Node node);
    }
}