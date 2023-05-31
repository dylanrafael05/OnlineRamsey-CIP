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

            var maxPath = paths[0];

            foreach(var p in paths) 
            {
                if(p.Length > maxPath.Length)
                {
                    maxPath = p;
                }
            }
            
            Debug.Log($"{maxPath.Start} -> {maxPath.End} ; length {maxPath.Length} [{Convert.ToString((long)maxPath.Mask, 2).PadLeft(64, '-')}]");
        }
    }

    public readonly struct VirtualEdgeCreateAction 
    {
        public VirtualEdgeCreateAction(VirtualEdge edge, Biconnection conn) 
        {
            Edge = edge;
            Connection = conn;
        }

        public VirtualEdge Edge { get; }
        public Biconnection Connection { get; }
    }

    public readonly struct Biconnection : IEquatable<Biconnection>
    {
        public Biconnection(int a, int b) 
        {
            Min = math.min(a, b);
            Max = math.max(a, b);
        }

        public Biconnection(Biconnection a, Biconnection b) 
        {
            Min = math.min(a.Min, b.Min);
            Max = math.max(a.Max, b.Max);
        }

        public int Min { get; }
        public int Max { get; }

        public bool Equals(Biconnection other)
        {
            return Min == other.Min && Max == other.Max;
        }

        public override int GetHashCode()
        {
            return Min + short.MaxValue * Max;
        }
    }

    public struct VirtualEdge
    {
        public ulong Mask;

        public static bool operator ==(VirtualEdge a, VirtualEdge b) 
            => a.Mask == b.Mask;
        public static bool operator !=(VirtualEdge a, VirtualEdge b) 
            => a.Mask != b.Mask;

        public int Count => math.countbits(Mask);
        public bool IsValid => Mask != 0;
        public static bool CheckValidCombo(VirtualEdge a, VirtualEdge b) 
            => math.countbits(a.Mask & b.Mask) == 1L;

        public static VirtualEdge operator |(VirtualEdge a, VirtualEdge b)
            => new() { Mask = a.Mask | b.Mask };

        public override string ToString()
        {
            return Convert.ToString((long)Mask, 2).PadLeft(8, '-');
        }

        public override bool Equals(object obj)
        {
            return obj is VirtualEdge v && this == v;
        }

        public override int GetHashCode()
        {
            return Mask.GetHashCode();
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

    public struct NativeListMatrix<T> : IDisposable 
        where T : unmanaged
    {
        public NativeListMatrix(int w, int h, int d, Allocator alloc)
        {
            Width = w;
            Height = h;
            DepthCapacity = d;

            array = new NativeArray<T>(w * h * d, alloc);
            depths = new NativeArray<int>(w * h, alloc);
        }

        public NativeListMatrix(NativeListMatrix<T> other) 
        {
            array = other.array;
            depths = other.depths;

            Width = other.Width;
            Height = other.Height;
            DepthCapacity = other.DepthCapacity;
        }

        [NativeDisableParallelForRestriction] private NativeArray<T> array;
        [NativeDisableParallelForRestriction] private NativeArray<int> depths;
        public int Width { get; }
        public int Height { get; }

        public int DepthCapacity { get; }

        public int BuildIndex(int x, int y, int z)
        {
            return y * DepthCapacity * Width 
                 + x * DepthCapacity 
                 + z;
        }

        public int DepthAt(int x, int y)
        {
            return depths[x * Width + y];
        }

        public bool Add(int x, int y, T value)
        {
            var di = x * Width + y;

            this[x, y, depths[di]] = value; 
            return ++depths[di] < DepthCapacity;
        }

        public T this[int x, int y, int z]
        {
            get => array[BuildIndex(x, y, z)];
            set => array[BuildIndex(x, y, z)] = value;
        }

        public void Dispose()
        {
            array.Dispose();
            depths.Dispose();
        }
    }

    public static class PathFinder 
    {
        public static Path[] FindAll(Graph graph) 
        {
            var nodeCount = graph.Nodes.Count;

            NativeListMatrix<VirtualEdge> matrix = new(nodeCount, nodeCount, 80, Allocator.Persistent);
            NativeQueue<VirtualEdgeCreateAction> actionQueue = new(Allocator.Persistent);

            var core = new PathFinderCoreJob
            {
                input = matrix,
                output = actionQueue.AsParallelWriter()
            };
            var merge = new PathFinderMergeJob
            {
                input = actionQueue,
                output = matrix
            };

            var anyChanges = true;

            foreach(var edge in graph.Edges)
            {
                matrix.Add(
                    math.min(edge.Start.ID, edge.End.ID), 
                    math.max(edge.Start.ID, edge.End.ID), 
                    new() { Mask = (1ul << edge.Start.ID) | (1ul << edge.End.ID) });
            }

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

            var count = 0;

            while(anyChanges)
            {
                var handle = core.Schedule(nodeCount * nodeCount, 16);
                handle.Complete();

                anyChanges = actionQueue.Count > 0;

                handle = merge.Schedule();
                handle.Complete();
                
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
                count++;

                if(count > 100) break;
            }

            var pathlist = new List<Path>();

            for(int i = 0; i < matrix.Width; i++)
            {
                for(int j = 0; j < matrix.Height; j++)
                {
                    for(int k = 0; k < matrix.DepthCapacity; k++)
                    {
                        var ve = matrix[i, j, k];

                        if(ve.IsValid)
                        {
                            pathlist.Add(new Path(ve, i, j));
                        }
                    }
                }
            }

            matrix.Dispose();
            actionQueue.Dispose();

            return pathlist.ToArray();
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct PathFinderMergeJob : IJob 
    {
        public NativeQueue<VirtualEdgeCreateAction> input;
        public NativeListMatrix<VirtualEdge> output;

        public void Execute()
        {
            while(input.TryDequeue(out var action))
            {
                var conn = action.Connection;
                var edge = action.Edge;
                
                var shouldcontinue = false;

                for(int i = 0; i < output.DepthAt(conn.Min, conn.Max); i++)
                {
                    var other = output[conn.Min, conn.Max, i];
                    if(other == edge || other.Count > edge.Count) 
                    {
                        shouldcontinue = true;
                        break;
                    }
                }

                if(shouldcontinue) continue;
                
                output.Add(conn.Min, conn.Max, edge);

                // Debug.Log($"{action.Connection.Min} -> {action.Connection.Max}, {action.Edge}");
            }
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct PathFinderCoreJob : IJobParallelFor
    {
        [ReadOnly, NativeDisableParallelForRestriction] public NativeListMatrix<VirtualEdge> input;

        [WriteOnly, NativeDisableParallelForRestriction] public NativeQueue<VirtualEdgeCreateAction>.ParallelWriter output;

        public void Execute(int i)
        {
            int2 p = new(i % input.Width, i / input.Width);
            if(p.x >= p.y) return;

            Biconnection t = new(p.x, p.y);
            
            for(int z = 0; z < input.DepthAt(p.x, p.y); z++)
            {
                VirtualEdge a = input[p.x, p.y, z];
                if (!a.IsValid) return;

                for(int y = p.x + 1; y < input.Height; y++)
                {
                    Biconnection o = new(p.x, y);

                    for (int zz = 0; zz < input.DepthAt(o.Min, o.Max); zz++)
                    {
                        VirtualEdge b = input[o.Min, o.Max, zz];
                        if (!b.IsValid) break;

                        if(VirtualEdge.CheckValidCombo(a, b)) 
                            output.Enqueue(new(a | b, new(t, o)));
                    }
                }
            }
        }
    }
}