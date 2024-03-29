using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

using Random = Unity.Mathematics.Random;
using System.Linq;
using UnityEngine;
using Ramsey.Graph.Experimental;
using Ramsey.Utilities;
using Unity.Burst;

namespace Ramsey.Graph.Experimental
{
    public struct NodeSmoothingPhysicsJob : IJobParallelFor
    {
        [ReadOnly] public NativeBitMatrix matrix;
        [NativeMatchesParallelForLength, ReadOnly] public NativeArray<float2> positions;
        [NativeMatchesParallelForLength, WriteOnly] public NativeArray<float2> outPositions;

        public static float GetScaleFromRadiusSquared(float r2)
        {
            return math.exp(-r2) + 4f * math.max(0, 2f - r2 * 0.5f);
        }

        public static float2 ModifyVelocity(float2 vel) 
        {
            const float A = 0.1f / math.PI;
            return math.atan(math.length(vel / A)) * A * math.normalizesafe(vel);
        }

        public void Execute(int index)
        {
            var pos = positions[index];
            var vel = new float2();

            var ran = Random.CreateFromIndex(math.asuint(index));

            for(int i = 0; i < positions.Length; i++)
            {
                if(i == index) continue;
                var other = positions[i];

                var delta = pos - other;
                var lendel = math.lengthsq(delta);

                var areconnected = matrix[i, index];

                if(lendel == 0)
                {
                    lendel = 0.01f;
                    delta = ran.NextFloat2Direction();
                }

                var scale = 0.3f * GetScaleFromRadiusSquared(lendel * 0.6f);
                if(areconnected)
                {
                    scale -= 3.0f * math.clamp(math.log(lendel) - 2.8f, -0.2f, 0.6f);
                }

                if(math.abs(scale) > 0.005f)
                {
                    vel += delta / math.length(delta) * scale;
                }
            }

            outPositions[index] = pos + ModifyVelocity(vel);
        }
    }
}