using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel;
using Unity.Mathematics;
using Unity.Burst.Intrinsics;
using Ramsey.Utilities;

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
            JobPathInternal p = input[i];
            
            var newChangesPresent = false;

            var w = matrix.Width;

            var pcur = p.End;
            var pother = p.Start;
    
            for(int other = 0; other < w; other++)
            {
                if(other == pcur || other == pother) continue;

                // Debug.Log($"Trying {min} -> {max}");
                bool b = matrix[pcur, other];
                if (!b) continue;

                // Debug.Log($"Checking {min} -> {max}");
                
                var othermask = (Bit256)1 << other;
                var newmask = p.Mask | othermask;

                if(newmask != p.Mask) 
                {
                    newChangesPresent = true;
                    output.Enqueue(new(newmask, p.Length + 1, pother, other, true));
                }
            }

            pcur = p.Start;
            pother = p.End;

            for(int other = 0; other < w; other++)
            {
                if(other == pcur || other == pother) continue;

                bool b = matrix[pcur, other];
                if (!b) continue;
                
                var othermask = (Bit256)1 << other;
                var newmask = p.Mask | othermask;

                if(newmask != p.Mask) 
                {
                    newChangesPresent = true;
                    output.Enqueue(new(newmask, p.Length + 1, pother, other, true));
                }
            }
            
            if(!newChangesPresent)
            {
                output.Enqueue(new(p, false));
            }
            else
            {
                anyChanges.Value = true;
            }
        }
    }
}