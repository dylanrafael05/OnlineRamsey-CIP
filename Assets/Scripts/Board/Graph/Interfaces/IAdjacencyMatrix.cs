namespace Ramsey.Graph
{
    public interface IAdjacencyMatrix : IReadOnlyAdjacencyMatrix
    {
        void Expand(int newNodeCount);
        void AddAdjacency(Node start, Node end);
    }
}