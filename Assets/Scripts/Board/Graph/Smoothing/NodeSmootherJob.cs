using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

using Random = Unity.Mathematics.Random;
using System.Linq;
using UnityEngine;

namespace Ramsey.Graph
{
    public struct NodeSmootherJob : IJobParallelFor
    {
        [NativeMatchesParallelForLength, ReadOnly] public NativeArray<float2> positions;
        [NativeMatchesParallelForLength, WriteOnly] public NativeArray<float2> outPositions;

        public void Execute(int index)
        {
            var pos = positions[index];
            var npos = pos;

            var ran = Random.CreateFromIndex(math.asuint(index));

            for(int i = 0; i < positions.Length; i++)
            {
                if(i == index) continue;
                var other = positions[i];

                var delta = pos - other;
                var lendel = math.lengthsq(delta);

                if(lendel == 0)
                {
                    lendel = 0.01f;
                    delta = ran.NextFloat2Direction();
                }

                lendel *= 1f;
                var scale = 0.5f * math.exp(-lendel*lendel);

                if(math.abs(scale) > 0.005f)
                {
                    npos += delta / math.length(delta) * scale;
                }
            }

            outPositions[index] = npos;
        }
    }
}