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

        public void HandlePaintedEdge(Edge edge, Graph graph)
        {
            // Prepare type
            var type = edge.Type;
            EnsurePathTypeAvailable(type);

            // Find incremental
            var (pathsInternalThisType, maxpath) = JobPathFinderImpl.FindIncr(graph, maxPaths[type], pathsInternal[type], edge);

            // Update data
            pathsInternal[type] = pathsInternalThisType;
            this.graph = graph;

            maxPaths[type] = new JobPath(maxpath, type, graph);
        }

        public void Clear()
        {
            pathsInternal.Clear();
            maxPaths.Clear();
        }
    }
}