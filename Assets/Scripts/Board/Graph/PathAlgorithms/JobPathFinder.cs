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
using System;

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
                Debug.Log($"{p.Start} -> {p.End} ; length {p.Length} [{Convert.ToString((long)p.Mask, 2).PadLeft(64, '-')}]");
            }
        }
    }

    public struct VirtualEdge
    {
        public ulong Mask;

        public int Count => X86.Popcnt.popcnt_u64(Mask);
        public bool IsValid => Mask != 0;
        public static bool CheckValidCombo(VirtualEdge a, VirtualEdge b) 
            => X86.Popcnt.popcnt_u64(a.Mask & b.Mask) == 1L;

        public static VirtualEdge operator |(VirtualEdge a, VirtualEdge b)
            => new VirtualEdge() { Mask = a.Mask | b.Mask };

        public override string ToString()
        {
            return Convert.ToString((long)Mask, 2).PadLeft(8, '-');
        }
    }

    public readonly struct Path 
    {
        public Path(VirtualEdge edge, int start, int end) 
        {
            Mask = edge.Mask;
            Length = edge.Count;
            Start = start;
            End = end;
        }

        public int Length { get; }
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
        }

        public NativeArray3D(NativeArray3D<T> other) 
        {
            array = other.array;

            Width = other.Width;
            Height = other.Height;
            Depth = other.Depth;
        }

        [NativeDisableParallelForRestriction] private NativeArray<T> array;
        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }

        public NativeArray<T> Array => array;

        public int BuildIndex(int x, int y, int z)
        {
            return y * Depth * Width 
                 + x * Depth 
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

            NativeArray3D<VirtualEdge> bufferA = new(nodeCount, nodeCount, 64, Allocator.Persistent);
            NativeArray3D<VirtualEdge> bufferB = new(nodeCount, nodeCount, 64, Allocator.Persistent);

            var job = new PathFinderJob
            {
                inputEdges = bufferA,
                outputEdges = bufferB
            };

            var anyChanges = true;
            NativeArray3D<VirtualEdge> currentOutput = bufferB;

            foreach(var edge in graph.Edges)
            {
                bufferA[
                    Mathf.Min(edge.Start.ID, edge.End.ID), 
                    Mathf.Max(edge.Start.ID, edge.End.ID), 
                    0
                ] = new() { Mask = (1ul << edge.Start.ID) | (1ul << edge.End.ID) };
            }
            
            bufferB.Array.CopyFrom(bufferA.Array);

            // var s = "";

            // for (int i = 0; i < job.outputEdges.Width; i++)
            // {
            //     for (int j = 0; j < job.outputEdges.Height; j++)
            //     {
            //         s += job.outputEdges[i, j, 0] + ", ";
            //     }
            //     s += "\n";
            // }
            
            // Debug.Log(s);

            while(anyChanges)
            {
                job.anyChanges = false;

                var handle = job.Schedule(nodeCount * nodeCount, 16);
                handle.Complete();

                anyChanges = job.anyChanges;

                // Debug.Log("Did it once, hyay!");

                // s = "";

                // for (int i = 0; i < job.outputEdges.Width; i++)
                // {
                //     for (int j = 0; j < job.outputEdges.Height; j++)
                //     {
                //         s += job.outputEdges[i, j, 0] + ", ";
                //     }
                //     s += "\n";
                // }

                // Debug.Log(s);


                if(anyChanges)
                {
                    job.inputEdges.Array.CopyFrom(job.outputEdges.Array);

                    (job.inputEdges, job.outputEdges) = (job.outputEdges, job.inputEdges);
                    currentOutput = job.outputEdges;
                }
            }

            var pathlist = new List<Path>();

            for(int i = 0; i < currentOutput.Width; i++)
            {
                for(int j = 0; j < currentOutput.Height; j++)
                {
                    for(int k = 0; k < currentOutput.Depth; k++)
                    {
                        var ve = currentOutput[i, j, k];

                        // Debug.Log($"Checking {i}, {j}, {k}");

                        if(ve.IsValid)
                        {
                            pathlist.Add(new Path(ve, i, j));
                        }
                    }
                }
            }

            bufferA.Array.Dispose();
            bufferB.Array.Dispose();

            return pathlist.ToArray();
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct PathFinderJob : IJobParallelFor
    {
        [ReadOnly, NativeDisableParallelForRestriction] public NativeArray3D<VirtualEdge> inputEdges;

        [WriteOnly, NativeDisableParallelForRestriction] public NativeArray3D<VirtualEdge> outputEdges;

        [WriteOnly] public bool anyChanges;

        public void Execute(int i)
        {
            int foundCount = 0;

            int2 p = new(i % inputEdges.Width, i / inputEdges.Width);

            // Debug.Log($"{p}");
            // return;

            if(p.x > p.y) return;

            for(int y = 0; y<inputEdges.Height; y++)
            {
                for(int z = 0; z<inputEdges.Depth; z++)
                {
                    VirtualEdge a = inputEdges[p.x, p.y, z];
                    if (!a.IsValid) break;

                    for (int zz = 0; zz < inputEdges.Depth; zz++)
                    {
                        VirtualEdge b = inputEdges[p.y, y, zz];
                        if (!b.IsValid) break;

                        if(VirtualEdge.CheckValidCombo(a, b)) 
                            outputEdges[p.x, y, foundCount] = a | b;
                        
                        foundCount++;
                        anyChanges = true;
                    }
                }
            }
        }
    }
}