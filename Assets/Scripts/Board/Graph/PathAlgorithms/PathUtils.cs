using System;
using System.Collections.Generic;
using System.Linq;

namespace Ramsey.Graph
{
    /// <summary>
    /// Implemented generalized algorithms to extrapolate information
    /// about any given path.
    /// </summary>
    internal static class PathUtils
    {
        public static IEnumerable<Edge> GetEdgesConnecting(IEnumerable<Node> nodes) 
        {
            Node last = null;
            foreach (var node in nodes.ToList())
            {
                if (last != null)
                {
                    yield return node.EdgeConnectedTo(last)
                        ?? throw new InvalidOperationException("Edges must connect the nodes in a path.");
                }

                last = node;
            }
        }

        public static IEnumerable<DirectedEdge> GetDirectedEdgesConnecting(IEnumerable<Node> nodes) 
        {
            Node last = null;
            foreach (var node in nodes.ToList())
            {
                if (last != null)
                {
                    yield return node.DirectedEdgeConnectedTo(last)
                        ?? throw new InvalidOperationException("Edges must connect the nodes in a path.");
                }

                last = node;
            }
        }
    }
}