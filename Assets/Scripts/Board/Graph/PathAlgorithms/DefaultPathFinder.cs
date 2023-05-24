using Ramsey.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ramsey.Graph
{
    public class DefaultPathFinder : IIncrementalPathFinder
    {
        public IEnumerable<Path> AllPaths => PathsByEndpoint.Values.Merge();
        private readonly List<Path> maxLengthPathByType;

        internal Dictionary<Node, HashSet<Path>> PathsByEndpoint { get; private set; }
        public IReadOnlyList<Path> MaxPathsByType => maxLengthPathByType;

        public DefaultPathFinder()
        {
            PathsByEndpoint = new();
        }
        internal DefaultPathFinder(Dictionary<Node, HashSet<Path>> pathsByEndpoint, List<Path> maxLengthPath)
        {
            PathsByEndpoint = pathsByEndpoint;
            maxLengthPathByType = maxLengthPath;
        }

        public void HandleNodeAddition(Node node)
        {
            // TODO: figure out how to add for each color specifid by user settings!
            PathsByEndpoint.Add(node, new() { new Path(node, 0), new Path(node, 1) });
        }

        public Task HandlePaintedEdge(Edge edge)
        {
            Assert.AreNotEqual(edge.Type, Edge.NullType, "Cannot add a non-painted edge to an incremental path finder!");

            var byStart = PathsByEndpoint[edge.Start].ToArray();
            var byEnd = PathsByEndpoint[edge.End].ToArray();

            // Remove those which did not start here
            PathsByEndpoint[edge.Start].RemoveWhere(p => p.Start != edge.Start);
            PathsByEndpoint[edge.End]  .RemoveWhere(p => p.Start != edge.End);

            foreach (var path in byStart)
                TryExpandPath(path, edge.Start);
            foreach (var path in byEnd)
                TryExpandPath(path, edge.End);

            return Task.CompletedTask;
        }

        internal void Clear()
        {
            maxLengthPathByType.Clear();
            PathsByEndpoint.Clear();
        }

        private void TryExpandPath(Path path, Node expandFrom)
        {
            Assert.IsTrue(path.IsEndpoint(expandFrom), "Cannot expand a path from a non-endpoint");

            var anyExpansions = false;

            Debug.Log("expanding path . . . ");

            // Iterate over all expansions
            foreach (var edge in expandFrom.ConnectedEdges)
            {
                Debug.Log("iterating edges . . . ");
                if (edge.Type != path.Type)
                {
                    continue;
                }

                var opposingNode = edge.NodeOpposite(expandFrom);

                if (path.Contains(opposingNode))
                {
                    continue;
                }

                anyExpansions = true;
                TryExpandPath(
                    path.Expand(opposingNode, expandFrom),
                    opposingNode
                );
            }

            // Add this to the max path if necessary
            if (!anyExpansions)
            {
                PathsByEndpoint[expandFrom].Add(path);

                // Expand size if needed
                while(path.Type > maxLengthPathByType.Count)
                {
                    Debug.Log("expanding max path count . . . ");
                    maxLengthPathByType.Add(null);
                }

                // Store if this is the maximum path length found
                if (MaxPathsByType[path.Type] == null || path.Length > MaxPathsByType[path.Type].Length)
                {
                    maxLengthPathByType[path.Type] = path;
                }
            }
        }
    }

    public class RemoveDuplicatesPathFinder : IIncrementalPathFinder
    {
        public IEnumerable<Path> AllPaths => PathByEndpoint.Values.Merge();
        private readonly List<Path> maxLengthPathByType;

        internal Dictionary<Node, List<Path>> PathByEndpoint { get; private set; }
        public IReadOnlyList<Path> MaxPathsByType => maxLengthPathByType;

        public RemoveDuplicatesPathFinder()
        {
            PathByEndpoint = new();
        }
        internal RemoveDuplicatesPathFinder(Dictionary<Node, List<Path>> pathsByEndpoint, List<Path> maxLengthPath)
        {
            PathByEndpoint = pathsByEndpoint;
            maxLengthPathByType = maxLengthPath;
        }

        public void HandleNodeAddition(Node node)
        {
            // TODO: figure out how to add for each color specifid by user settings!
            PathByEndpoint.Add(node, new() { new Path(node, 0), new Path(node, 1) });
        }

        public Task HandlePaintedEdge(Edge edge)
        {
            Assert.AreNotEqual(edge.Type, Edge.NullType, "Cannot add a non-painted edge to an incremental path finder!");

            var byStart = PathByEndpoint[edge.Start].ToArray();
            var byEnd = PathByEndpoint[edge.End].ToArray();

            PathByEndpoint[edge.Start].Clear();
            PathByEndpoint[edge.End].Clear();

            foreach (var path in byStart)
                TryExpandPath(path, edge.Start);
            foreach (var path in byEnd)
                TryExpandPath(path, edge.End);

            return Task.CompletedTask;
        }

        internal void Clear()
        {
            maxLengthPathByType.Clear();
            PathByEndpoint.Clear();
        }

        private void TryExpandPath(Path path, Node expandFrom)
        {
            Assert.IsTrue(path.IsEndpoint(expandFrom), "Cannot expand a path from a non-endpoint");

            var anyExpansions = false;

            // Iterate over all expansions
            foreach (var edge in expandFrom.ConnectedEdges)
            {
                if (edge.Type != path.Type)
                {
                    continue;
                }

                var opposingNode = edge.NodeOpposite(path.End);

                if (path.Contains(opposingNode))
                {
                    continue;
                }

                anyExpansions = true;
                TryExpandPath(
                    path.Expand(opposingNode, expandFrom),
                    opposingNode
                );
            }

            // Add this to the max path if necessary
            if (!anyExpansions)
            {
                foreach(var p in PathByEndpoint[expandFrom])
                {
                    if(p != path && p.IsIdenticalTo(path))
                    {
                        return;
                    }
                }

                PathByEndpoint[expandFrom].Add(path);

                // Expand size if needed
                while(path.Type > maxLengthPathByType.Count)
                {
                    maxLengthPathByType.Add(null);
                }

                // Store if this is the maximum path length found
                if (MaxPathsByType[path.Type] == null || path.Length > MaxPathsByType[path.Type].Length)
                {
                    maxLengthPathByType[path.Type] = path;
                }
            }
        }
    }
}