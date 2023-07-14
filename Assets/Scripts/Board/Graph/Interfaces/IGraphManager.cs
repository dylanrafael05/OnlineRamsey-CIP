using Unity.Mathematics;

namespace Ramsey.Graph
{
    /// <summary>
    /// Represents any object which wraps control
    /// of a graph.
    /// </summary>
    public interface IGraphManager
    {
        IReadOnlyGraph Graph { get; }

        void MoveNode(Node node, float2 position);
        int? MaxNodeCount { get; }

        void LoadGraph(IGraph graph);
    }
}