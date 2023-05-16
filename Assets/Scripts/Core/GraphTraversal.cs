using System.Collections.Generic;
using System.Linq;

namespace Ramsey.Core
{
    public static class GraphTraversal
    {
        internal static HashSet<Node> BestPathStartingAt(Node startpoint, int type, HashSet<Node> existing = null)
        {
            if(existing is null)
                existing = new();

            var best = existing;

            foreach(var edge in startpoint.Edges)
            {   
                if(edge.Type != type)
                    continue;
                
                var neighbor = edge.NodeOpposite(startpoint);

                if(existing.Contains(neighbor))
                    continue;

                var withNeighbor = existing.ToHashSet();
                withNeighbor.Add(neighbor);

                var bestFromNeighbor = BestPathStartingAt(neighbor, type, withNeighbor);

                if(best.Count < bestFromNeighbor.Count)
                    best = bestFromNeighbor;
            }

            return best;
        }

        public static (IEnumerable<Node> path, int color) BestPath(GraphManager graph)
        {
            HashSet<Node> best = null;
            int color = 0;

            // NOTE: this shoyuld be dynamic later
            for(int type = 0; type < 2; type++)
            {
                foreach(var node in graph.Nodes)
                {
                    var bestFromNode = BestPathStartingAt(node, type);

                    if(best is null || best.Count < bestFromNode.Count)
                    {
                        best = bestFromNode;
                        color = type;
                    }
                }
            }

            return (best, color);
        }
    }
}