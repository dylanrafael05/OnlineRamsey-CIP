using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using System.Linq;
using Ramsey.Utilities;
using Ramsey.Graph.Experimental;

namespace Ramsey.Graph.Experimental
{
    public static class NodeSmoothing
    {
        public static void Smooth(IGraphManager gm, int iterationCount = 1)
        {
            var positions = new NativeArray<float2>(
                gm.Graph.Nodes.Select(n => n.Position).ToArray(),
                Allocator.TempJob);
            var outPositions = new NativeArray<float2>(positions, Allocator.TempJob);
            var matrix = gm.Graph.TotalAdjacencies.ToNative(Allocator.TempJob);

            var ns = new NodeSmootherJob
            {
                matrix = matrix,
                positions = positions,
                outPositions = outPositions,
            };

            for(int count = 0; count < iterationCount; count++)
            {
                (ns.outPositions, ns.positions) = (ns.positions, ns.outPositions);
                ns.Run(positions.Length);
            }

            for (int i = 0; i < ns.outPositions.Length; i++)
            {
                gm.MoveNode(gm.Graph.Nodes[i], ns.outPositions[i]);
            }

            positions.Dispose();
            outPositions.Dispose();
            matrix.Dispose();
        }
    }
}