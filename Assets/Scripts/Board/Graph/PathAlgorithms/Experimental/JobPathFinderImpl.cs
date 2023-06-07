using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using System.Linq;
using Ramsey.Utilities;
using System.Collections.Generic;

namespace Ramsey.Graph.Experimental
{
    internal static class JobPathFinderImpl
    {
        internal static JobPathInternal[] FindAll(Graph graph, int type)
        {
            var nodeCount = graph.Nodes.Count;

            NativeBitMatrix matrix = graph.AdjacenciesForType(type).ToNative(Allocator.Persistent);
            NativeList<JobPathInternal> livePaths = new(nodeCount * nodeCount, Allocator.Persistent);
            NativeHashSet<JobPathInternal> deadPaths = new(nodeCount * nodeCount, Allocator.Persistent);
            
            foreach(var edge in graph.Edges.Where(e => e.Type == type))
            {
                matrix[edge.Start.ID, edge.End.ID] = true;
                matrix[edge.End.ID, edge.Start.ID] = true;

                var p = new JobPathInternal
                (
                    (Bit256.One << edge.Start.ID) | (Bit256.One << edge.End.ID),
                    1,
                    edge.Start.ID,
                    edge.End.ID
                );

                livePaths.Add(p);
            }

            return Find(matrix, livePaths, deadPaths);
        }

        internal static JobPathInternal[] FindIncr(Graph graph, IReadOnlyList<JobPath> existingPaths, Edge newEdge)
        {
            NativeBitMatrix matrix = graph.AdjacenciesForType(newEdge.Type).ToNative(Allocator.Persistent);
            NativeList<JobPathInternal> livePaths = new(existingPaths.Count * 2 + 1, Allocator.Persistent);
            NativeHashSet<JobPathInternal> deadPaths = new(existingPaths.Count * 2 + 1, Allocator.Persistent);

            foreach(var p in existingPaths)
            {
                if(p.Start == newEdge.Start || p.Start == newEdge.End || p.End == newEdge.Start || p.End == newEdge.End)
                {
                    livePaths.AddNoResize(p.Internal);
                }
                else 
                {
                    deadPaths.Add(p.Internal);
                }
            }

            var edgepath = new JobPathInternal
            (
                (Bit256.One << newEdge.Start.ID) | (Bit256.One << newEdge.End.ID),
                1,
                newEdge.Start.ID,
                newEdge.End.ID
            );
            
            livePaths.Add(edgepath);

            return Find(matrix, livePaths, deadPaths);
        }

        internal static JobPathInternal[] Find(NativeBitMatrix matrix, NativeList<JobPathInternal> startLivePaths, NativeHashSet<JobPathInternal> startDeadPaths) 
        {
            NativeList<JobPathInternal> livePaths = startLivePaths;
            NativeHashSet<JobPathInternal> deadPaths = startDeadPaths;

            NativeQueue<JobPathGeneration> generationQueue = new(Allocator.Persistent);
            NativeReturn<bool> anyChanges = new(Allocator.Persistent, true);
            
            var gen = new PathGenerateJob
            {
                matrix = matrix,
                input = livePaths.AsParallelReader(),
                output = generationQueue.AsParallelWriter(),
                anyChanges = anyChanges
            };
            var agg = new PathAggregateJob
            {
                input = generationQueue,
                deadOutput = deadPaths,
                liveOutput = livePaths
            };

            while(livePaths.Length > 0)
            {
                anyChanges.Value = false;

                gen.input = livePaths.AsParallelReader();
                gen.output = generationQueue.AsParallelWriter();
                gen.matrix = matrix;

                gen.Schedule(livePaths.Length, 16).Complete();
                agg.Schedule().Complete();
            }

            var pathlistn = deadPaths.ToNativeArray(Allocator.TempJob);
            var pathlist = pathlistn.ToArray();
            pathlistn.Dispose();

            matrix.Dispose();
            deadPaths.Dispose();
            livePaths.Dispose();
            generationQueue.Dispose();
            anyChanges.Dispose();

            return pathlist;
        }
    }
}