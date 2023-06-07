using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ramsey.Utilities;
using UnityEngine;

namespace Ramsey.Graph.Experimental
{
    public class JobPathFinder : IIncrementalPathFinder
    {
        private readonly List<JobPath[]> paths = new();
        private readonly List<IPath> maxPaths = new();

        int? IIncrementalPathFinder.MaxSupportedNodeCount => 256;

        public IEnumerable<IPath> AllPaths => paths.Merge().Select(t => (IPath)t);
        public IReadOnlyList<IPath> MaxPathsByType => maxPaths;

        public void HandleNodeAddition(Node node)
        {}

        private void EnsurePathTypeAvailable(int type)
        {
            maxPaths.PadDefaultUpto(type);
            paths.PadUpto(type, () => new JobPath[0]);
        }

        public async Task HandlePaintedEdge(Edge edge, Graph graph)
        {
            // Prepare type
            var type = edge.Type;
            EnsurePathTypeAvailable(type);

            // Find all
            var pathsInternal = await Task.Run(() => JobPathFinderImpl.FindIncr(graph, paths[type], edge));
            // var pathsInternal = await Task.Run(() => JobPathFinderImpl.FindAll(graph, type));

            // Update data
            paths[type] = new JobPath[pathsInternal.Length];
            maxPaths[type] = null;

            var block = new CellBlock<List<Node>>(pathsInternal.Length);

            for(int i = 0; i < pathsInternal.Length; i++) 
            {
                var p = pathsInternal[i];
                var jp = new JobPath(p, block.GetCell(i), graph, type);

                if(maxPaths[type] == null || p.Length > maxPaths[type].Length)
                {
                    maxPaths[type] = jp;
                }

                paths[type][i] = jp;
            }
        }
    }
}