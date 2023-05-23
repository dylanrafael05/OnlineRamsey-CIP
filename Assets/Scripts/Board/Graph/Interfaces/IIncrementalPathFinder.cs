using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ramsey.Graph
{
    public interface IIncrementalPathFinder
    {
        IEnumerable<IPath> AllPaths { get; }
        IPath MaxLengthPath { get; }

        void HandleNodeAddition(Node node);
        Task HandlePaintedEdge(Edge edge);
    }
}