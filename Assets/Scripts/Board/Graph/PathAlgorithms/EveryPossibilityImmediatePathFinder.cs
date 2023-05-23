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

        internal Dictionary<Node, List<IPath>> NodesByTerminatingPaths { get; private set; }
        public IPath MaxLengthPath { get; private set; }

        public DefaultPathFinder()
        {
            NodesByTerminatingPaths = new();
        }
        internal DefaultPathFinder(Dictionary<Node, List<IPath>> nodesByTerminatingPaths, IPath maxLengthPath)
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

            foreach (var path in byStart)
                ExpandPath(path);
            foreach (var path in byEnd)
                ExpandPath(path);

            return Task.CompletedTask;
        }

        internal void Clear()
        {
            MaxLengthPath = null;
            NodesByTerminatingPaths.Clear();
        }

        private void ExpandPath(IPath path)
        {
            var any = false;

            // Debug.Log("[" + string.Join(", ", ((Path)path).nodes.Select(n => n.ID)) + "]");

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

            if (!any)
            {
                NodesByTerminatingPaths[path.End].Add(path);

                if (MaxLengthPath == null || path.Length > MaxLengthPath.Length)
                {
                    MaxLengthPath = path;
                }
            }
        }
    }
}