using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using System.Linq;
using Ramsey.Utilities;
using Ramsey.Graph.Experimental;
using System;

namespace Ramsey.Graph.Experimental
{
    public static class NodeSmoothing
    {
        private const float MaxWaitSyncMillis = 10f;
        private const int RepeatCount = 2;

        private static JobHandle? handle;

        private static NativeArray<float2> positions;
        private static NativeArray<float2> outPositions;
        private static NativeBitMatrix matrix;

        private static void ScheduleJob(IGraphManager gm)
        {
            positions = new NativeArray<float2>(
                gm.Graph.Nodes.Select(n => n.Position).ToArray(),
                Allocator.Persistent);
            
            outPositions = new NativeArray<float2>(positions, Allocator.Persistent);
            matrix = gm.Graph.GetNativeAdjacencyMatrix(Allocator.Persistent);

            var physicsJob = new NodeSmoothingPhysicsJob
            {
                positions = positions,
                outPositions = outPositions,
                matrix = matrix
            };

            handle = physicsJob.Schedule(positions.Length, 32);
        }

        private static void CompleteJob(IGraphManager gm)
        {
            handle.Value.Complete();

            for (int i = 0; i < outPositions.Length; i++)
            {
                gm.MoveNode(gm.Graph.Nodes[i], outPositions[i]);
            }

            matrix.Dispose();
            positions.Dispose();
            outPositions.Dispose();
        }

        public static void Update(IGraphManager gm)
        {
            // Schedule new job if necessary
            if(handle is null)
            {
                ScheduleJob(gm);
            }

            var startTime = DateTime.Now;
            var count = 0;

            while(true)
            {
                if(DateTime.Now.Millisecond - startTime.Millisecond >= MaxWaitSyncMillis)
                {
                    // Timeout
                    return;
                }

                if(handle.Value.IsCompleted)
                {
                    // Complete job
                    CompleteJob(gm);

                    count++;

                    // Restart job if necessary
                    if(count == RepeatCount)
                    {
                        handle = null;
                        return;
                    }
                    else 
                    {
                        ScheduleJob(gm);
                    }
                }
            }
        }
    }
}