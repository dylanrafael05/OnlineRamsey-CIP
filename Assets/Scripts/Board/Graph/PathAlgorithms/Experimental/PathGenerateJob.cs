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
        [ReadOnly, NativeDisableParallelForRestriction] public NativeBitMatrix matrix;

        [ReadOnly, NativeMatchesParallelForLength] public NativeArray<JobPathInternal>.ReadOnly input;
        [WriteOnly] public NativeQueue<JobPathGeneration>.ParallelWriter output;
        [WriteOnly, NativeDisableParallelForRestriction] public NativeReturn<bool> anyChanges;

        public void Execute(int i)
        {
            JobPathInternal path = input[i];
            
            var newChangesPresent = false;

            var w = matrix.Width;

            var pcur = path.End;
            var pother = path.Start;
    
            for(int other = 0; other < w; other++)
            {
                if(other == pcur || other == pother) continue;

                // Debug.Log($"Trying {min} -> {max}");
                bool b = matrix[pcur, other];
                if (!b) continue;

                // Debug.Log($"Checking {min} -> {max}");
                
                var othermask = (Bit256)1 << other;
                var newmask = path.Mask | othermask;

                if(newmask != path.Mask) 
                {
                    newChangesPresent = true;
                    output.Enqueue(new(newmask, path.Length + 1, pother, other, true));
                }
            }

            pcur = path.Start;
            pother = path.End;

            for(int other = 0; other < w; other++)
            {
                if(other == pcur || other == pother) continue;

                bool b = matrix[pcur, other];
                if (!b) continue;
                
                var othermask = (Bit256)1 << other;
                var newmask = path.Mask | othermask;

                if(newmask != path.Mask) 
                {
                    newChangesPresent = true;
                    output.Enqueue(new(newmask, path.Length + 1, pother, other, true));
                }
            }
            
            if(newChangesPresent)
            {
                anyChanges.Value = true;
            }
            
            output.Enqueue(new(path, false));
        }
    }
}