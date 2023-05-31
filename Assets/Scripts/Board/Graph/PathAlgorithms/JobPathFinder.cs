using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel;
using Unity.Mathematics;
using Unity.Burst.Intrinsics;

namespace Ramsey.Graph.Experimental
{
    public struct VirtualEdge
    {
        public ulong Mask;

        public int Count => X86.Popcnt.popcnt_u64(Mask);
        public bool Exists => Count != 0;
        public static bool CheckValidCombo(VirtualEdge a, VirtualEdge b) 
            => X86.Popcnt.popcnt_u64(a.Mask & b.Mask) == 1;

        public static VirtualEdge operator |(VirtualEdge a, VirtualEdge b)
            => new VirtualEdge() { Mask = a.Mask | b.Mask };
    }

    public struct NativeArray3D<T> where T : unmanaged
    {
        public NativeArray3D(int w, int h, int d, Allocator alloc)
        {
            array = new NativeArray<T>(w * h * d, alloc);

            Width = w;
            Height = h;
            Depth = d;
        }

        private NativeArray<T> array;
        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }

        public NativeArray<T> Array => array;

        public T this[int x, int y, int z]
        {
            get => array[y * Depth * Height + x * Depth + z];
            set => array[y * Depth * Height + x * Depth + z] = value;
        }

    }

    [BurstCompile(CompileSynchronously = true)]
    public struct PathFinderJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray3D<VirtualEdge> inputEdges;

        [WriteOnly] public NativeArray3D<VirtualEdge> outputEdges;

        public void Execute(int i)
        {
            int foundCount = 0;

            int2 p = new(i % inputEdges.Width, i / inputEdges.Width);

            for(int y = 0; y<inputEdges.Height; y++)
            {
                if (inputEdges[p.y, y, 0].Count != 0)
                {
                    for(int z = 0; z<inputEdges.Depth; z++)
                    {
                        if (!inputEdges[p.x, p.y, z].Exists) break;
                        VirtualEdge a = inputEdges[p.x, p.y, z];
                        for (int zz = 0; zz < inputEdges.Depth; zz++)
                        {
                            if (!inputEdges[p.y, y, zz].Exists) break;
                            VirtualEdge b = inputEdges[p.y, y, zz];
                            if(VirtualEdge.CheckValidCombo(a, b)) outputEdges[p.x, y, foundCount] = a | inputEdges[p.y, y, zz];
                            foundCount++;
                        }
                    }
                }
            }
        }
    }
}