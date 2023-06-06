using Unity.Mathematics;

namespace Ramsey.Graph
{
    public interface IGraph : IReadOnlyGraph
    {
        Node CreateNode(float2 position = default);
        Edge CreateEdge(Node start, Node end);

        void PaintEdge(Edge edge, int type);
        void MoveNode(Node node, float2 position);

        void Clear();
    }
}