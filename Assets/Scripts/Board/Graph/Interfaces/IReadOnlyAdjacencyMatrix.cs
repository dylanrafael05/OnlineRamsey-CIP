namespace Ramsey.Graph
{
    public interface IReadOnlyAdjacencyMatrix
    {
        bool AreAdjacent(Node start, Node end);
        bool AreAdjacent(int start, int end);

        int Size { get; }
    }
}