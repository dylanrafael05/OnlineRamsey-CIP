using System.Collections.Generic;
using System.Threading.Tasks;
using Ramsey.Utilities;
using UnityEngine;

namespace Ramsey.Graph.Experimental
{
    public class JobPathFinder : IIncrementalPathFinder
    {
        private readonly List<List<JobPath>> paths = new();
        private readonly List<IPath> maxPaths = new();

        int? IIncrementalPathFinder.MaxSupportedNodeCount => 256;

        public IEnumerable<IPath> AllPaths => paths.Merge();
        public IReadOnlyList<IPath> MaxPathsByType => maxPaths;

        public void HandleNodeAddition(Node node)
        {}

        private void EnsurePathTypeAvailable(int type)
        {
            maxPaths.PadDefaultUpto(type);
            paths.PadNewUpto(type);
        }

        public async Task HandlePaintedEdge(Edge edge, Graph graph)
        {
            // Prepare type
            var type = edge.Type;
            EnsurePathTypeAvailable(type);

            // Find all
            var pathsInternal = await Task.Run(() => JobPathFinderImpl.FindIncr(graph, paths[type], edge));

            // Update data
            paths[type].Clear();
            maxPaths[type] = null;

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