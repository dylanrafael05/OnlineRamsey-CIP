using System.Collections.Generic;

namespace Ramsey.Graph
{
    /// <summary>
    /// Represents a monochromatic path of edges.
    /// </summary>
    public interface IPath
    {
        /// <summary>
        /// The type/color o this path.
        /// </summary>
        int Type { get; }
        /// <summary>
        /// The list of nodes, in order, which comprise this path.
        /// </summary>
        IEnumerable<Node> Nodes { get; }

        /// <summary>
        /// The length of this path. This is equal to the number
        /// of edges which comprise the path.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// The first node in this path.
        /// </summary>
        Node Start { get; }
        /// <summary>
        /// The middle node in this path.
        /// </summary>
        Node Middle { get; }
        /// <summary>
        /// The last node in this path.
        /// </summary>
        Node End { get; }

        /// <summary>
        /// The edges which comprise this path.
        /// </summary>
        IEnumerable<Edge> Edges { get; }
        /// <summary>
        /// The edges which comprise this path, directed
        /// such that the end of one edge will be the start of the next.
        /// </summary>
        IEnumerable<DirectedEdge> DirectedEdges { get; }

        /// <summary>
        /// Check if this path contains the given node.
        /// </summary>
        bool Contains(Node node);
        /// <summary>
        /// CHeck if the given node is an endpoint of this path.s
        /// </summary>
        bool IsEndpoint(Node node);
    }
}