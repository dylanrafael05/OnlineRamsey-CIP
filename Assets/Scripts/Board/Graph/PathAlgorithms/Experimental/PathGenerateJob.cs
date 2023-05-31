using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel;
using Unity.Mathematics;
using Unity.Burst.Intrinsics;

namespace Ramsey.Graph.Experimental
{
    [BurstCompile(CompileSynchronously = true)]
    internal struct PathGenerateJob : IJobParallelFor
    {
        [ReadOnly, NativeDisableParallelForRestriction] public NativeBitMatrix matrix;
        [ReadOnly] public int step;

        [ReadOnly, NativeMatchesParallelForLength] public NativeArray<JobPathInternal>.ReadOnly input;
        [WriteOnly] public NativeQueue<JobPathInternal>.ParallelWriter output;
        [WriteOnly, NativeDisableParallelForRestriction] public NativeReturn<bool> anyChanges;

        public void Execute(int i)
        {
            JobPathInternal p = input[i];

            if(p.Length < step) return;

            var pcur = p.End;
            var pother = p.Start;

            var newChangesPresent = false;

            var w = matrix.Width;
    
            for(int other = 0; other < w; other++)
            {
                if(other == pcur || other == pother) continue;

                var min = math.min(pcur, other);
                var max = math.max(pcur, other);

                // Debug.Log($"Trying {min} -> {max}");
                bool b = matrix[min, max];
                if (!b) continue;

                // Debug.Log($"Checking {min} -> {max}");
                
                var othermask = 1UL << other;
                var newmask = p.Mask | othermask;

                if(newmask != p.Mask) 
                {
                    newChangesPresent = true;
                    output.Enqueue(new(newmask, p.Length + 1, pother, other));
                }
            }

            pcur = p.Start;
            pother = p.End;

            for(int other = 0; other < w; other++)
            {
                if(other == pcur || other == pother) continue;

                var min = math.min(pcur, other);
                var max = math.max(pcur, other);

                // Debug.Log($"Trying {min} -> {max}");
                bool b = matrix[min, max];
                if (!b) continue;
                
                // Debug.Log($"Checking {min} -> {max}");
                
                var othermask = 1UL << other;
                var newmask = p.Mask | othermask;

                if(newmask != p.Mask) 
                {
                    newChangesPresent = true;
                    output.Enqueue(new(newmask, p.Length + 1, pother, other));
                }
            }
            
            if(!newChangesPresent)
            {
                output.Enqueue(p);
            }
            else
            {
                anyChanges.Value = true;
            }
        }
    }
}