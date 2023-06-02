using Unity.Mathematics;

namespace Ramsey.Graph
{
    public interface IGraphManager
    {
        IReadOnlyGraph Graph { get; }
        void MoveNode(Node node, float2 position);
    }
}