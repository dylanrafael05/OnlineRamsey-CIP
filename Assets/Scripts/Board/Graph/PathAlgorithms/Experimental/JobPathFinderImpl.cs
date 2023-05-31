using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using System.Linq;

namespace Ramsey.Graph.Experimental
{
    internal static class JobPathFinderImpl
    {
        internal static JobPathInternal[] FindAll(Graph graph, int type) 
        {
            var nodeCount = graph.Nodes.Count;

            NativeBitMatrix matrix = new(nodeCount, nodeCount, Allocator.Persistent);
            NativeList<JobPathInternal> deadPaths = new(nodeCount * nodeCount, Allocator.Persistent);
            NativeList<JobPathInternal> livePaths = new(nodeCount * nodeCount, Allocator.Persistent);
            NativeQueue<JobPathInternal> actionQueue = new(Allocator.Persistent);
            NativeReturn<bool> anyChangesWrap = new(Allocator.Persistent);

            foreach(var edge in graph.Edges.Where(e => e.Type == type))
            {
                var min = math.min(edge.Start.ID, edge.End.ID);
                var max = math.max(edge.Start.ID, edge.End.ID);

                matrix[min, max] = true;

                var p = new JobPathInternal
                (
                    (1UL << min) | (1UL << max),
                    1,
                    min,
                    max
                );

                livePaths.Add(p);
            }

            // var s = "";
            // for(int j = 0; j < matrix.Height; j++)
            // {
            //     s += j + ": ";
            //     for(int i = 0; i < matrix.Width; i++)
            //     {
            //         s += matrix[i, j] ? '#' : '-';
            //     }
            //     s += "\n";
            // }
            // Debug.Log(s);
            
            var core = new PathGenerateJob
            {
                matrix = matrix,
                input = livePaths.AsParallelReader(),
                output = actionQueue.AsParallelWriter(),
                anyChanges = anyChangesWrap
            };
            var merge = new PathAggregateJob
            {
                input = actionQueue,
                deadOutput = deadPaths,
                liveOutput = livePaths
            };

            var anyChanges = true;
            var step = 1;

            while(anyChanges)
            {
                anyChangesWrap.Value = false;

                core.output = actionQueue.AsParallelWriter();
                core.input = livePaths.AsParallelReader();
                core.matrix = matrix;

                core.step = step;

                var handle = core.Schedule(livePaths.Length, 16);
                handle.Complete();

                anyChanges = anyChangesWrap.Value;

                merge.step = step;

                handle = merge.Schedule();
                handle.Complete();

                step++;

                if(step > 100) break;
            }

            var pathlist = deadPaths.ToArray();

            matrix.Dispose();
            deadPaths.Dispose();
            livePaths.Dispose();
            actionQueue.Dispose();
            anyChangesWrap.Dispose();

            return pathlist;
        }
    }
}