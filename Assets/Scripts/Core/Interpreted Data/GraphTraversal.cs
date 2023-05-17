using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Ramsey.Core
{
    public class Path 
    {
        private Path(Path path) 
        {
            nodes = new HashSet<Node>(path.nodes);
            End = path.End;
            Type = path.Type;
        }

        public Path(Node node, int type) 
        {
            nodes = new() {node};
            End = node;
            Type = type;
        }

        private HashSet<Node> nodes;

        public int Type { get; }

        public IReadOnlyCollection<Node> Nodes => nodes;
        public int Length => nodes.Count;

        public Node End { get; private set; }

        public bool Contains(Node node) 
        {
            return nodes.Contains(node);
        }
        public bool Add(Node node) 
        {
            if(Contains(node)) return false;

            nodes.Add(node);
            End = node;

            return true;
        }

        public Path Append(Node node) 
        {
            var other = new Path(this);
            other.Add(node);

            return other;
        }
    }

    public class IncrementalPathFinder
    {
        private Dictionary<Node, List<Path>> nodesByTerminatingPaths;
        private Path maximumLength = null;

        public IncrementalPathFinder()
        {
            nodesByTerminatingPaths = new();
        }

        public void AddNode(Node node) 
        {
            nodesByTerminatingPaths.Add(node, new() {new Path(node, 0), new Path(node, 1)});
        }

        public void AddEdge(Edge edge)
        {
            foreach(var path in nodesByTerminatingPaths[edge.Start])
                ExpandPath(path);
            foreach(var path in nodesByTerminatingPaths[edge.End])
                ExpandPath(path);

            nodesByTerminatingPaths[edge.Start].Clear();
            nodesByTerminatingPaths[edge.End].Clear();
        }

        private void ExpandPath(Path path) 
        {
            var any = false;

            foreach(var edge in path.End.Edges)
            {
                if(edge.Type != path.Type)
                {
                    continue;
                }
                
                var opposingNode = edge.NodeOpposite(path.End);
                
                if(path.Contains(opposingNode))
                {
                    continue;
                }

                any = true;
                ExpandPath(path.Append(opposingNode));
            }

            if(!any)
            {
                nodesByTerminatingPaths[path.End].Add(path);

                if(path.Length > maximumLength.Length)
                {
                    maximumLength = path;
                }
            }
        }
    }

    public static class GraphTraversal
    {
        internal static HashSet<Node> BestPathStartingAt(Node startpoint, int type, HashSet<Node> existing = null)
        {
            if(existing is null)
                existing = new() {startpoint};

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