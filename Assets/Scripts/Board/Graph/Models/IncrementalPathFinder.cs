using Ramsey.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Ramsey.Graph
{
    public class IncrementalPathFinder
    {
        internal IEnumerable<Path> AllPaths => NodesByTerminatingPaths.Values.Merge();

        internal Dictionary<Node, List<Path>> NodesByTerminatingPaths { get; private set; }
        public Path MaxLengthPath { get; private set; }

        public IncrementalPathFinder()
        {
            NodesByTerminatingPaths = new();
        }
        internal IncrementalPathFinder(Dictionary<Node, List<Path>> nodesByTerminatingPaths, Path maxLengthPath)
        {
            NodesByTerminatingPaths = nodesByTerminatingPaths;
            MaxLengthPath = maxLengthPath;
        }

        public void HandleNodeAddition(Node node) 
        {
            // TODO: figure out how to add for each color specifid by user settings!
            NodesByTerminatingPaths.Add(node, new() {new Path(node, 0), new Path(node, 1)});
        }

        public void HandlePaintedEdge(Edge edge)
        {
            Assert.AreEqual(edge.Type, -1, "Cannot add a non-painted edge to an incremental path finder!");

            foreach(var path in NodesByTerminatingPaths[edge.Start])
                ExpandPath(path);
            foreach(var path in NodesByTerminatingPaths[edge.End])
                ExpandPath(path);

            NodesByTerminatingPaths[edge.Start].Clear();
            NodesByTerminatingPaths[edge.End].Clear();
        }

        internal void Clear()
        {
            MaxLengthPath = null;
            NodesByTerminatingPaths.Clear();
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
                NodesByTerminatingPaths[path.End].Add(path);

                if(path.Length > MaxLengthPath.Length)
                {
                    MaxLengthPath = path;
                }
            }
        }
    }
}