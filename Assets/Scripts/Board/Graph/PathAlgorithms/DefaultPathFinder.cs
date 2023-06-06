using Ramsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Ramsey.Graph
{
    public class DefaultPathFinder : IIncrementalPathFinder
    {
        public IEnumerable<IPath> AllPaths => PathsByColorByEndpoint.Values.Merge().Merge();
        private readonly List<Path> maxLengthPathByType;

        internal Dictionary<Node, List<HashSet<Path>>> PathsByColorByEndpoint { get; private set; }
        public IReadOnlyList<IPath> MaxPathsByType => maxLengthPathByType;

        int? IIncrementalPathFinder.MaxSupportedNodeCount => null;

        public DefaultPathFinder()
        {
            PathsByColorByEndpoint = new();
            maxLengthPathByType = new();
        }
        internal DefaultPathFinder(Dictionary<Node, List<HashSet<Path>>> pathsByEndpoint, List<Path> maxLengthPath)
        {
            PathsByColorByEndpoint = pathsByEndpoint;
            maxLengthPathByType = maxLengthPath;
        }

        public void HandleNodeAddition(Node node)
        {
            PathsByColorByEndpoint.Add(node, new() { new() { new Path(node, 0) }, new() { new Path(node, 1) }});
        }

        public Task HandlePaintedEdge(Edge edge, Graph _)
        {
            Assert.AreNotEqual(edge.Type, Edge.NullType, "Cannot add a non-painted edge to an incremental path finder!");

            var byStart = PathsByColorByEndpoint[edge.Start][edge.Type].ToArray();
            var byEnd = PathsByColorByEndpoint[edge.End][edge.Type].ToArray();

            // Remove those which did not start here
            PathsByColorByEndpoint[edge.Start][edge.Type].Clear();
            PathsByColorByEndpoint[edge.End][edge.Type].Clear();

            foreach (var path in byStart)
            {
                TryExpandPath(path, true);
            }
            foreach (var path in byEnd)
            {
                TryExpandPath(path, false);
            }

            return Task.CompletedTask;
        }

        internal void Clear()
        {
            maxLengthPathByType.Clear();
            PathsByColorByEndpoint.Clear();
        }

        private void TryExpandPath(Path path, bool prepend)
        {
            var expandFrom = prepend ? path.Start : path.End;
            var anyExpansions = false;

            // Iterate over all expansions
            foreach (var edge in expandFrom.ConnectedEdgesOfType(path.Type))
            {
                var opposingNode = edge.NodeOpposite(expandFrom);

                if (path.Contains(opposingNode))
                {
                    continue; 
                }

                anyExpansions = true;
                TryExpandPath(
                    prepend ? path.Prepend(opposingNode) : path.Append(opposingNode),
                    prepend
                );
            }

            // Add this to the max path if necessary
            if (!anyExpansions)
            {
                PathsByColorByEndpoint[path.Start][path.Type].Add(path);
                PathsByColorByEndpoint[path.End][path.Type].Add(path);

                // Expand size if needed
                while(path.Type >= maxLengthPathByType.Count)
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