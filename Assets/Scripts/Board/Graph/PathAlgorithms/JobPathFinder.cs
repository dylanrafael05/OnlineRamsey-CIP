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
        public static bool CheckValidCombo(VirtualEdge a, VirtualEdge b) 
            => X86.Popcnt.popcnt_u64(a.Mask & b.Mask) == 1;
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
            get => array[x * Depth * Height + y * Depth + z];
            set => array[x * Depth * Height + y * Depth + z] = value;
        }

    }

    [BurstCompile(CompileSynchronously = true)]
    public struct PathFinderJob : IJobParallelFor
    {
        public NativeArray3D<VirtualEdge> edges;

        public void Execute(int i)
        {
            throw new System.NotImplementedException();
        }
    }
}