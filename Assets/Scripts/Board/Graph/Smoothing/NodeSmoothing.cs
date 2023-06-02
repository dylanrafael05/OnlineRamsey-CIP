using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using System.Linq;

namespace Ramsey.Graph
{
    public static class NodeSmoothing
    {
        public static void Smooth(IGraphManager gm, int iterationCount = 1)
        {
            var positions = new NativeArray<float2>(
                gm.Graph.Nodes.Select(n => n.Position).ToArray(),
                Allocator.TempJob);

            var ns = new NodeSmootherJob
            {
                positions = positions,
            };

            for(int count = 0; count < iterationCount; count++)
            {
                ns.Run(positions.Length);
            }

            for(int i = 0; i < positions.Length; i++)
            {
                gm.MoveNode(gm.Graph.Nodes[i], positions[i]);
            }

            positions.Dispose();
        }
    }
}