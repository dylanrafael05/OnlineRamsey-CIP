using System.Linq;
using Unity.Collections;

namespace Ramsey.Graph.Experimental
{
    /// <summary>
    /// Implements functions to gain access to native information
    /// about a graph's adjacencies.
    /// </summary>
    public static class AdjacencyUtils
    {
        public static NativeAdjacencyList GetNativeAdjacencyList(this IReadOnlyGraph g, Allocator alloc, int? type = null)
        {
            var nodes = g.Nodes.Select(n => n.ID).ToList();
            var mlen = g.Nodes.Max(n => n.NeighborCount);

            var list = new NativeAdjacencyList(g.Nodes.Count, mlen, alloc);

            for(int i = 0; i < nodes.Count; i++)
            {
                var n = g.NodeFromID(i);

                foreach(var neighbor in n.Neighbors)
                {
                    if(n.IsConnectedTo(neighbor) && (!type.HasValue || n.EdgeConnectedTo(neighbor).Type == type))
                    {
                        list.Add((byte)i, (byte)neighbor.ID);
                    }
                }
            }

            return list;
        }

        public static NativeBitMatrix GetNativeAdjacencyMatrix(this IReadOnlyGraph g, Allocator alloc, int? type = null)
        {
            var am = new NativeBitMatrix(g.Nodes.Count, g.Nodes.Count, alloc);

            foreach(var n1 in g.Nodes)
            {
                foreach(var n2 in g.Nodes)
                {
                    am[n1.ID, n2.ID] = n1.IsConnectedTo(n2) && (!type.HasValue || n1.EdgeConnectedTo(n2).Type == type);
                }
            }
            
            return am;
        }
    }
}