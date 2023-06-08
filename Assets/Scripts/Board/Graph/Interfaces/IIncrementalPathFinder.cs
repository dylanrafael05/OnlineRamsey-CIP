using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ramsey.Graph
{
    public interface IIncrementalPathFinder
    {
        IEnumerable<IPath> AllPaths { get; }
        IReadOnlyList<IPath> MaxPathsByType { get; }

        void HandleNodeAddition(Node node);
        void HandlePaintedEdge(Edge edge, Graph graph);

        int? MaxSupportedNodeCount { get; }

        void Clear();
    }
}