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

        public float GetScaleFromRadiusSquared(float r2)
        {
            return math.exp(-r2);
        }

        public float2 ModifyVelocity(float2 vel) 
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

                if(lendel == 0)
                {
                    lendel = 0.01f;
                    delta = ran.NextFloat2Direction();
                }

                var scale = 0.15f * GetScaleFromRadiusSquared(lendel * 0.6f);

                if(math.abs(scale) > 0.005f)
                {
                    vel += delta / math.length(delta) * scale;
                }
            }

            outPositions[index] = pos + ModifyVelocity(vel);
        }
    }
}