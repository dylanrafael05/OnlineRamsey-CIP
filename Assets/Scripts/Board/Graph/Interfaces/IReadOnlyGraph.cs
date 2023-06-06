using System.Collections.Generic;
using Unity.Mathematics;

namespace Ramsey.Graph
{
    public interface IReadOnlyGraph
    {
        IReadOnlyList<Node> Nodes {get;}
        IReadOnlyList<Edge> Edges {get;}
        
        IReadOnlyAdjacencyMatrix AdjacenciesForType(int type);
        IReadOnlyAdjacencyMatrix TotalAdjacencies { get; }

        bool IsValidEdge(Node start, Node end);
        bool IsComplete();
        bool IsCompleteColored();

        Node NodeFromID(int id);
        Edge EdgeFromID(int id);
    }
}