using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel;
using Unity.Mathematics;
using Unity.Burst.Intrinsics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Ramsey.Graph.Experimental
{
    public class JobsPathFinder : IIncrementalPathFinder
    {
        public IEnumerable<Ramsey.Graph.Path> AllPaths => Enumerable.Empty<Ramsey.Graph.Path>();

        public IReadOnlyList<Ramsey.Graph.Path> MaxPathsByType => new List<Ramsey.Graph.Path>();

        public void HandleNodeAddition(Node node)
        {}

        public async Task HandlePaintedEdge(Edge edge, Graph graph)
        {
            var paths = await Task.Run(() => PathFinder.FindAll(graph));

            foreach(var p in paths) 
            {
                Debug.Log($"{p.Start} -> {p.End} [{p.Mask}]");
            }
        }
    }

    public struct VirtualEdge
    {
        public ulong Mask;

        public bool IsValid => Mask != 0;

        public int Count => X86.Popcnt.popcnt_u64(Mask);
        public static bool CheckValidCombo(VirtualEdge a, VirtualEdge b) 
            => X86.Popcnt.popcnt_u64(a.Mask & b.Mask) == 1;
    }

    public readonly struct Path 
    {
        public Path(VirtualEdge edge, int start, int end) 
        {
            Mask = edge.Mask;
            Start = start;
            End = end;
        }

        public ulong Mask { get; }
        public int Start { get; }
        public int End { get; }
    }

    public struct NativeArray3D<T> where T : unmanaged
    {
        public NativeArray3D(int w, int h, int d, Allocator alloc)
        {
            array = new NativeArray<T>(w * h * d, alloc);

            Width = w;
            Height = h;
            Depth = d;

            InvertBuffer = false;
        }

        public NativeArray3D(NativeArray3D<T> other) 
        {
            array = other.array;

            Width = other.Width;
            Height = other.Height;
            Depth = other.Depth;
            
            InvertBuffer = false;
        }

        private NativeArray<T> array;
        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }

        public bool InvertBuffer { get; set; }

        public NativeArray<T> Array => array;

        public int BuildIndex(int x, int y, int z)
        {
            return (InvertBuffer ? Width - x : x) * Depth * Height 
                + (InvertBuffer ? Height - y : y) * Depth 
                + z;
        }

        public T this[int x, int y, int z]
        {
            get => array[BuildIndex(x, y, z)];
            set => array[BuildIndex(x, y, z)] = value;
        }

    }

    public static class PathFinder 
    {
        public static Path[] FindAll(Graph graph) 
        {
            var nodeCount = graph.Nodes.Count;

            NativeArray3D<VirtualEdge> bufferA = new(nodeCount, nodeCount, 64, Allocator.TempJob);
            NativeArray3D<VirtualEdge> bufferB = new(bufferA)
            {
                InvertBuffer = true
            };

            var job = new PathFinderJob
            {
                inputEdges = bufferA,
                outputEdges = bufferB
            };

            var anyChanges = true;

            foreach(var edge in graph.Edges)
            {
                bufferA[
                    Mathf.Min(edge.Start.ID, edge.End.ID), 
                    Mathf.Max(edge.Start.ID, edge.End.ID), 
                    0
                ] = new() { Mask = (1ul << edge.Start.ID) | (1ul << edge.End.ID) };
            }

            while(anyChanges)
            {
                var handle = job.Schedule(nodeCount * nodeCount, 16);
                handle.Complete();

                if(anyChanges)
                {
                    job.inputEdges.InvertBuffer  = !job.inputEdges.InvertBuffer;
                    job.outputEdges.InvertBuffer = !job.outputEdges.InvertBuffer;
                }
            }

            var pathlist = new List<Path>();

            for(int i = 0; i < bufferA.Width; i++)
            {
                for(int j = i + 1; j < bufferA.Height; j++)
                {
                    for(int k = 0; k < bufferA.Depth; k++)
                    {
                        var ve = job.outputEdges[i, j, k];

                        if(ve.IsValid)
                        {
                            pathlist.Add(new Path(ve, i, j));
                        }
                    }
                }
            }

            return pathlist.ToArray();
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct PathFinderJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray3D<VirtualEdge> inputEdges;
        [WriteOnly]
        public NativeArray3D<VirtualEdge> outputEdges;
        public bool anyChanges;

        public void Execute(int i)
        {
            throw new System.NotImplementedException();
        }
    }
}