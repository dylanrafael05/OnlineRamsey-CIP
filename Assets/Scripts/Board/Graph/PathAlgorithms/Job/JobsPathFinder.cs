using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Ramsey.Utilities;
using Unity.Profiling;
using UnityEngine;

namespace Ramsey.Graph.Experimental
{
    /// <summary>
    /// A class which handles the pre and post processing
    /// of the jobs path finding approach, as well as implements
    /// the appropriate incremental path finder interface.
    /// <br/>
    /// 
    /// This path finder has a hard limit of 256 nodes due to the
    /// 256-bit representation used for a path's contained nodes.
    /// <br/>
    /// 
    /// To increase this, one would need to expand the Bit256 struct
    /// to a larger variant, such as a Bit512 struct and so on. 
    /// Unfortunately, infinite nodes would seem to be impossible
    /// using Unity's Native code limitations.
    /// </summary>
    public class JobPathFinder : IIncrementalPathFinder
    {
        private readonly List<JobPathInternal[]> pathsInternal = new();
        private readonly List<JobPathInternal> maxPathsInternal = new();
        private readonly List<IPath> maxPaths = new();

        int? IIncrementalPathFinder.MaxSupportedNodeCount => 256;

        public IReadOnlyList<IPath> MaxPathsByType => maxPaths;

        public void HandleNodeAddition(Node node)
        {}

        private void EnsurePathTypeAvailable(int type)
        {
            maxPaths.PadDefaultUpto(type);
            maxPathsInternal.PadDefaultUpto(type);
            pathsInternal.PadUpto(type, () => new JobPathInternal[0]);
        }

        public void HandlePaintedEdge(Edge edge, IReadOnlyGraph graph)
        {
            // Prepare type
            var type = edge.Type;
            EnsurePathTypeAvailable(type);

            // Find incremental
            var (pathsInternalThisType, maxpath) = JobPathFinderImpl.FindIncr(graph, maxPathsInternal[type], pathsInternal[type], edge);

            // Update data
            pathsInternal[type] = pathsInternalThisType;

            maxPaths[type] = new JobPath(maxpath, type, graph);
            maxPathsInternal[type] = maxpath;
        }

        public void HandleFullGraph(IReadOnlyGraph graph)
        {
            Clear();

            EnsurePathTypeAvailable(graph.NumTypes);

            for(int type = 0; type < graph.NumTypes; type++)
            {
                var (pathsInternalThisType, maxpath) = JobPathFinderImpl.FindAll(graph, type);

                // Update data
                pathsInternal[type] = pathsInternalThisType;

                maxPaths[type] = new JobPath(maxpath, type, graph);
                maxPathsInternal[type] = maxpath;
            }
        }

        public void Clear()
        {
            pathsInternal.Clear();
            maxPaths.Clear();
            maxPathsInternal.Clear();
        }
    }
}