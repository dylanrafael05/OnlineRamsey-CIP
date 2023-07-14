using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using System.Linq;
using Ramsey.Utilities;
using System.Collections.Generic;
using Unity.Burst;

namespace Ramsey.Graph.Experimental
{
    /// <summary>
    /// Supplies the implementations of both all-at-once and one-at-a-time
    /// path finding types.
    /// </summary>
    internal static class JobPathFinderImpl
    {
        internal static (JobPathInternal[] paths, JobPathInternal longest) FindAll(IReadOnlyGraph graph, int type)
        {
            var nodeCount = graph.Nodes.Count;

            NativeAdjacencyList adjList = graph.GetNativeAdjacencyList(Allocator.Persistent, type);
            NativeList<JobPathInternal> livePaths = new(nodeCount * nodeCount, Allocator.Persistent);
            NativeHashSet<JobPathInternal> deadPaths = new(nodeCount * nodeCount, Allocator.Persistent);
            
            foreach(var edge in graph.Edges.Where(e => e.Type == type))
            {
                var p = new JobPathInternal
                (
                    (Bit256.One << edge.Start.ID) | (Bit256.One << edge.End.ID),
                    (byte)edge.Start.ID,
                    (byte)edge.End.ID
                );

                livePaths.Add(p);
            }

            return Find(adjList, livePaths, deadPaths, default, type);
        }

        internal static (JobPathInternal[] paths, JobPathInternal longest) FindIncr(IReadOnlyGraph graph, JobPathInternal longest, IReadOnlyList<JobPathInternal> existingPaths, Edge newEdge)
        {
            NativeAdjacencyList adjList = graph.GetNativeAdjacencyList(Allocator.Persistent, newEdge.Type);
            NativeList<JobPathInternal> livePaths = new(existingPaths.Count * 2 + 1, Allocator.Persistent);
            NativeHashSet<JobPathInternal> deadPaths = new(existingPaths.Count * 2 + 1, Allocator.Persistent);

            foreach(var p in existingPaths)
            {
                if(p.Start == newEdge.Start.ID || p.Start == newEdge.End.ID || p.End == newEdge.Start.ID || p.End == newEdge.End.ID)
                {
                    livePaths.AddNoResize(p);
                }
                else 
                {
                    deadPaths.Add(p);
                }
            }

            var edgepath = new JobPathInternal
            (
                (Bit256.One << newEdge.Start.ID) | (Bit256.One << newEdge.End.ID),
                (byte)newEdge.Start.ID,
                (byte)newEdge.End.ID
            );
            
            // if(newEdge.Start.NeighborCount == 1 && newEdge.End.NeighborCount == 1)
            livePaths.Add(edgepath); 
            // else deadPaths.Add(edgepath);

            return Find(adjList, livePaths, deadPaths, longest, newEdge.Type);
        }

        /// <summary>
        /// Manages the jobs and memory allocations
        /// necessary to find paths with the given start
        /// conditions.
        /// </summary>
        internal static (JobPathInternal[] paths, JobPathInternal longest) Find(NativeAdjacencyList adjacencies, NativeList<JobPathInternal> startLivePaths, NativeHashSet<JobPathInternal> startDeadPaths, JobPathInternal longest, int type) 
        {
            NativeList<JobPathInternal> livePaths = startLivePaths;
            NativeHashSet<JobPathInternal> deadPaths = startDeadPaths;

            NativeHashSet<JobPathInternal> liveSet = new(startLivePaths.Length, Allocator.Persistent);
            NativeQueue<JobPathGeneration> generationQueue = new(Allocator.Persistent);

            NativeValue<JobPathInternal> longestWrap = new(Allocator.Persistent, longest);
            
            var gen = new PathGenerateJob();
            var agg = new PathAggregateJob();

            while(livePaths.Length > 0)
            {
                gen.input = livePaths.AsParallelReader();
                gen.output = generationQueue.AsParallelWriter();
                gen.adjacencies = adjacencies;
                gen.type = type;

                gen.Schedule(livePaths.Length, 16).Complete();

                agg.input = generationQueue;
                agg.liveSet = liveSet;
                agg.liveOutput = livePaths;
                agg.deadOutput = deadPaths;
                agg.longest = longestWrap;

                agg.Schedule().Complete();

                longest = agg.longest.Value;
            }

            var pathlist = new JobPathInternal[deadPaths.Count()];
            var i = 0;

            var iter = deadPaths.GetEnumerator();
            while(iter.MoveNext())
            {
                pathlist[i++] = iter.Current;
            }

            adjacencies.Dispose();
            liveSet.Dispose();
            deadPaths.Dispose();
            livePaths.Dispose();
            generationQueue.Dispose();
            longestWrap.Dispose();

            return (pathlist, longest);
        }
    }
}