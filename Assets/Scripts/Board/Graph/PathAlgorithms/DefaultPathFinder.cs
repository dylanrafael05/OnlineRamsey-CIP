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
        public IEnumerable<IPath> AllPaths => NodesByTerminatingPaths.Values.Merge();
        private readonly List<IPath> maxLengthPathByType;

        internal Dictionary<Node, List<IPath>> NodesByTerminatingPaths { get; private set; }
        public IReadOnlyList<IPath> MaxPathsByType => maxLengthPathByType;

        public DefaultPathFinder()
        {
            NodesByTerminatingPaths = new();
        }
        internal DefaultPathFinder(Dictionary<Node, List<IPath>> nodesByTerminatingPaths, List<IPath> maxLengthPath)
        {
            NodesByTerminatingPaths = nodesByTerminatingPaths;
            maxLengthPathByType = maxLengthPath;
        }

        public void HandleNodeAddition(Node node)
        {
            // TODO: figure out how to add for each color specifid by user settings!
            NodesByTerminatingPaths.Add(node, new() { new Path(node, 0), new Path(node, 1) });
        }

        public Task HandlePaintedEdge(Edge edge)
        {
            Assert.AreNotEqual(edge.Type, Edge.NullType, "Cannot add a non-painted edge to an incremental path finder!");

            var byStart = NodesByTerminatingPaths[edge.Start].ToArray();
            var byEnd = NodesByTerminatingPaths[edge.End].ToArray();

            NodesByTerminatingPaths[edge.Start].Clear();
            NodesByTerminatingPaths[edge.End].Clear();

            foreach (var path in byStart)
                ExpandPath(path);
            foreach (var path in byEnd)
                ExpandPath(path);

            return Task.CompletedTask;
        }

        internal void Clear()
        {
            maxLengthPathByType.Clear();
            NodesByTerminatingPaths.Clear();
        }

        private void ExpandPath(IPath path)
        {
            var any = false;

            // Iterate over all expansions
            foreach (var edge in path.End.Edges)
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

                any = true;
                ExpandPath(path.Append(opposingNode));
            }

            // Add this to the max path if necessary
            if (!any)
            {
                NodesByTerminatingPaths[path.End].Add(path);

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