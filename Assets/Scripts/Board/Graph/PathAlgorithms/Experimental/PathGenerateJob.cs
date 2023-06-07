using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel;
using Unity.Mathematics;
using Unity.Burst.Intrinsics;
using Ramsey.Utilities;
using Unity.Profiling;

namespace Ramsey.Graph.Experimental
{
    [BurstCompile(CompileSynchronously = true)]
    internal struct PathGenerateJob : IJobParallelFor
    {
        [ReadOnly] public int type;
        [ReadOnly, NativeDisableParallelForRestriction] public NativeAdjacencyList adjacencies;

        [ReadOnly, NativeMatchesParallelForLength] public NativeArray<JobPathInternal>.ReadOnly input;
        [WriteOnly] public NativeQueue<JobPathGeneration>.ParallelWriter output;

        public void Execute(int i)
        {
            JobPathInternal path = input[i];

            var pcur = path.End;
            var pother = path.Start;
    
            var iter = adjacencies.GetIterator(pcur);

            while(iter.Move(out var other))
            {
                if(other == pcur | other == pother) continue;
                
                var othermask = (Bit256)1 << other;
                var newmask = path.Mask | othermask;

                if(newmask != path.Mask) 
                {
                    output.Enqueue(new(newmask, pother, other, true));
                }
            }

            pcur = path.Start;
            pother = path.End;

            iter = adjacencies.GetIterator(pcur);

            while(iter.Move(out var other))
            {
                if(other == pcur | other == pother) continue;
                
                var othermask = (Bit256)1 << other;
                var newmask = path.Mask | othermask;

                if(newmask != path.Mask) 
                {
                    output.Enqueue(new(newmask, pother, other, true));
                }
            }
            
            output.Enqueue(new(path, false));
        }
    }
}