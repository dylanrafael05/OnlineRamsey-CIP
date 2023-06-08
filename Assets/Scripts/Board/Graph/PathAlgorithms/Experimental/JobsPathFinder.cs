using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ramsey.Utilities;
using Unity.Profiling;
using UnityEngine;

namespace Ramsey.Graph.Experimental
{
    public class JobPathFinder : IIncrementalPathFinder
    {
        private Graph graph;

        private readonly List<JobPathInternal[]> pathsInternal = new();
        private readonly List<JobPath> maxPaths = new();

        int? IIncrementalPathFinder.MaxSupportedNodeCount => 256;

        public IEnumerable<IPath> AllPaths => pathsInternal.SelectMany((ps, i) => ps.Select(j => new JobPath(j, i, graph)));
        public IReadOnlyList<IPath> MaxPathsByType => maxPaths.Cast<IPath>().ToList();

        public void HandleNodeAddition(Node node)
        {}

        private void EnsurePathTypeAvailable(int type)
        {
            maxPaths.PadDefaultUpto(type);
            pathsInternal.PadUpto(type, () => new JobPathInternal[0]);
        }

        public async Task HandlePaintedEdge(Edge edge, Graph graph)
        {
            // Prepare type
            var type = edge.Type;
            EnsurePathTypeAvailable(type);

            // Find all
            var pathsInternalThisType = await Task.Run(() => JobPathFinderImpl.FindIncr(graph, pathsInternal[type], edge));
            // var pathsInternal = await Task.Run(() => JobPathFinderImpl.FindAll(graph, type));

            // Update data
            using(Collect.Auto()) 
            {
                pathsInternal[type] = pathsInternalThisType;

                var maxpath = (JobPathInternal?)null;

                for(int i = 0; i < pathsInternalThisType.Length; i++) 
                {
                    var p = pathsInternalThisType[i];

                    if(maxpath is null || p.Length > maxpath.Value.Length)
                    {
                        maxpath = p;
                    }
                }

                this.graph = graph;

                maxPaths[type] = new JobPath(maxpath.Value, type, graph);
            }
        }

        public void Clear()
        {
            pathsInternal.Clear();
            maxPaths.Clear();
        }

        private static ProfilerMarker Collect = new("JobPathFinder.Collect");
    }
}