using Unity.Mathematics;

namespace Ramsey.Graph
{
    /// <summary>
    /// A modifiable interface for a graph
    /// </summary>
    public interface IGraph : IReadOnlyGraph
    {
        /// <summary>
        /// Create a new node at the provided position.
        /// </summary>
        Node CreateNode(float2 position = default);
        /// <summary>
        /// Create a new edge connecting the two given nodes.
        /// This edge will be uncolored.
        /// </summary>
        Edge CreateEdge(Node start, Node end);

        /// <summary>
        /// Paint the given edge the given type.
        /// </summary>
        void PaintEdge(Edge edge, int type);
        /// <summary>
        /// Move the provided node to the provided new position.
        /// </summary>
        void MoveNode(Node node, float2 position);

        /// <summary>
        /// Remove all of the contents of this graph.
        /// </summary>
        void Clear();
    }
}