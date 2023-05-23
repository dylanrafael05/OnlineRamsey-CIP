using Ramsey.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Ramsey.Graph
{
    public class EveryPossibilityMergeLastPathFinder : IIncrementalPathFinder
    {
        public IEnumerable<IPath> AllPaths => NodesByTerminatingPaths.Values.Merge();

        private List<IPath> newPaths = new();

        internal Dictionary<Node, List<IPath>> NodesByTerminatingPaths { get; private set; }
        public IPath MaxLengthPath { get; private set; }

        public EveryPossibilityMergeLastPathFinder()
        {
            NodesByTerminatingPaths = new();
        }
        internal EveryPossibilityMergeLastPathFinder(Dictionary<Node, List<IPath>> nodesByTerminatingPaths, IPath maxLengthPath)
        {
            NodesByTerminatingPaths = nodesByTerminatingPaths;
            MaxLengthPath = maxLengthPath;
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

            var count = byStart.Length + byEnd.Length;

            newPaths.Clear(); // TODO: refine approximation

            foreach(var path in byStart)
                ExpandPath(newPaths, path);
            foreach(var path in byEnd)
                ExpandPath(newPaths, path);

            foreach(var path in newPaths)
            {
                NodesByTerminatingPaths[path.End].Add(path);

                if (MaxLengthPath == null || path.Length > MaxLengthPath.Length)
                {
                    MaxLengthPath = path;
                }
            }

            return Task.CompletedTask;
        }

        internal void Clear()
        {
            MaxLengthPath = null;
            NodesByTerminatingPaths.Clear();
        }

        private void ExpandPath(IList<IPath> newPaths, IPath path)
        {
            var startCount = newPaths.Count;

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

                ExpandPath(newPaths, path.Append(opposingNode));
            }

            if (newPaths.Count == startCount)
            {
                newPaths.Add(path);
            }
        }
    }
}