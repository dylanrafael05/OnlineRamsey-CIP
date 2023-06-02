using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

using Random = Unity.Mathematics.Random;
using System.Linq;

namespace Ramsey.Graph
{
    public struct NodeSmootherJob : IJobParallelFor
    {
        [NativeMatchesParallelForLength]
        public NativeArray<float2> positions;

        public static float GetScaleFromRadiusSquared(float r2)
        {
            return (math.pow(math.E, -r2) - .5f) / (r2*r2*r2 + 1);
        }

        public void Execute(int index)
        {
            var pos = positions[index];
            var npos = pos;

            var ran = Random.CreateFromIndex(math.asuint(index));

            for(int i = 0; i < positions.Length; i++)
            {
                var other = positions[i];

                var delta = pos - other;
                var lendel = math.lengthsq(delta);

                if(lendel == 0)
                {
                    lendel = 0.01f;
                    delta = ran.NextFloat2Direction();
                }

                var scale = GetScaleFromRadiusSquared(lendel);

                if(math.abs(scale) > 0.01f)
                {
                    npos += math.normalize(delta) * scale;
                }
            }

            positions[index] = npos;
        }
    }
}