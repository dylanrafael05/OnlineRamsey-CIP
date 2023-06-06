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
            NativeList<JobPathInternal> startPaths = new(nodeCount * nodeCount, Allocator.Persistent);
            
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

                startPaths.Add(p);
            }

            return Find(matrix, startPaths);
        }

        internal static JobPathInternal[] FindIncr(Graph graph, IReadOnlyList<JobPath> existingPaths, Edge newEdge)
        {
            NativeBitMatrix matrix = graph.AdjacenciesForType(newEdge.Type).ToNative(Allocator.Persistent);
            NativeList<JobPathInternal> startPaths = new(existingPaths.Count * 2 + 1, Allocator.Persistent);

            foreach(var p in existingPaths)
            {
                startPaths.AddNoResize(p.Internal);
            }

            startPaths.AddNoResize(new JobPathInternal
            (
                (Bit256.One << newEdge.Start.ID) | (Bit256.One << newEdge.End.ID),
                1,
                newEdge.Start.ID,
                newEdge.End.ID
            ));

            return Find(matrix, startPaths);
        }

        internal static JobPathInternal[] Find(NativeBitMatrix matrix, NativeList<JobPathInternal> startPaths) 
        {
            NativeList<JobPathInternal> livePaths = startPaths;
            NativeList<JobPathInternal> deadPaths = new(matrix.Area, Allocator.Persistent);

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

            while(anyChanges.Value)
            {
                anyChanges.Value = false;

                gen.input = livePaths.AsParallelReader();
                gen.output = generationQueue.AsParallelWriter();
                gen.matrix = matrix;

                gen.Schedule(livePaths.Length, 16).Complete();
                agg.Schedule().Complete();
            }

            var pathlist = deadPaths.ToArray();

            matrix.Dispose();
            deadPaths.Dispose();
            livePaths.Dispose();
            generationQueue.Dispose();
            anyChanges.Dispose();

            return pathlist;
        }
    }
}