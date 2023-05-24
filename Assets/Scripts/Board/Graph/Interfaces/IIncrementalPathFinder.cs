using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ramsey.Graph
{
    public interface IIncrementalPathFinder
    {
        IEnumerable<Path> AllPaths { get; }
        IReadOnlyList<Path> MaxPathsByType { get; }

        void HandleNodeAddition(Node node);
        Task HandlePaintedEdge(Edge edge);
    }
}