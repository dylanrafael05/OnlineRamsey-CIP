using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ramsey.Graph
{
    /// <summary>
    /// Represents any algorithm which incrementally
    /// finds the longest monochromatic path in a graph.
    /// </summary>
    public interface IIncrementalPathFinder
    {
        /// <summary>
        /// Gets a list of the longest paths indexed by the
        /// type they are colored by.
        /// </summary>
        IReadOnlyList<IPath> MaxPathsByType { get; }

        /// <summary>
        /// Updates internal state to account for a new node.
        /// </summary>
        void HandleNodeAddition(Node node);
        /// <summary>
        /// Updates internal state to account for a newly painted edge.
        /// </summary>
        void HandlePaintedEdge(Edge edge, IReadOnlyGraph graph);

        /// <summary>
        /// Updates internal state to account for a new graph produced
        /// either all at once or in a way which cannot reasonly be broken 
        /// down into node additions and edge paints.
        /// </summary>
        void HandleFullGraph(IReadOnlyGraph graph);

        /// <summary>
        /// The maximum number of nodes this path finder can 
        /// work with properly.
        /// </summary>
        int? MaxSupportedNodeCount { get; }

        /// <summary>
        /// Clear out any data within this path finder.
        /// </summary>
        void Clear();
    }
}