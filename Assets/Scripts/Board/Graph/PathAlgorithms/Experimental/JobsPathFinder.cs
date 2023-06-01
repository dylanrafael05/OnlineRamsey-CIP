using System.Collections.Generic;
using System.Threading.Tasks;
using Ramsey.Utilities;

namespace Ramsey.Graph.Experimental
{
    public class JobsPathFinder : IIncrementalPathFinder
    {
        private List<List<IPath>> paths = new();
        private List<IPath> maxPaths = new();

        public IEnumerable<IPath> AllPaths => paths.Merge();
        public IReadOnlyList<IPath> MaxPathsByType => maxPaths;

        public void HandleNodeAddition(Node node)
        {}

        private void EnsurePathTypeAvailable(int type)
        {
            while(maxPaths.Count <= type)
            {
                maxPaths.Add(null);
            }

            while(paths.Count <= type)
            {
                paths.Add(new());
            }
        }

        public async Task HandlePaintedEdge(Edge edge, Graph graph)
        {
            // Prepare type
            var type = edge.Type;
            EnsurePathTypeAvailable(type);

            paths[type].Clear();
            maxPaths[type] = null;

            // Find all
            var pathsInternal = await Task.Run(() => JobPathFinderImpl.FindAll(graph, type));

            // Get maximum path
            foreach(var p in pathsInternal) 
            {
                var jp = new JobPath(p, graph, type);

                if(maxPaths[type] == null || p.Length > maxPaths[type].Length)
                {
                    maxPaths[type] = jp;
                }

                paths[type].Add(jp);
            }
        }
    }
}