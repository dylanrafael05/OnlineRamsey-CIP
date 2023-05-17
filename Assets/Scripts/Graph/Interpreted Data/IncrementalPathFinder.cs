using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Ramsey.Core
{
    public class IncrementalPathFinder
    {
        private Dictionary<Node, List<Path>> nodesByTerminatingPaths;
        private Path maximumLength = null;

        public Path MaxLengthPath => maximumLength;

        public IncrementalPathFinder()
        {
            nodesByTerminatingPaths = new();
        }

        public void HandleNodeAddition(Node node) 
        {
            // TODO: figure out how to add for each color specifid by user settings!
            nodesByTerminatingPaths.Add(node, new() {new Path(node, 0), new Path(node, 1)});
        }

        public void HandlePaintedEdge(Edge edge)
        {
            Assert.AreEqual(edge.Type, -1, "Cannot add a non-painted edge to an incremental path finder!");

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
}