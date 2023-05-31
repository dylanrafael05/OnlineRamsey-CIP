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
        public Path(ulong mask, int count, int start, int end) 
        {
            Mask = mask;
            Length = count;
            Start = start;
            End = end;
        }

        public int Length { get; }
        public ulong Mask { get; }
        public int Start { get; }
        public int End { get; }

        public override string ToString()
        {
            return $"{Start} -> {End} ; length {Length} [{Convert.ToString((long)Mask, 2).PadLeft(64, '-')}]";
        }
    }

    public readonly struct NativeBitMatrix : IDisposable 
    {
        public NativeBitMatrix(int w, int h, Allocator alloc) 
        {
            Width = w;
            Height = h;
            Size = w * h;

            innerArray = new(Size, alloc);
        }

        private readonly NativeBitArray innerArray;
        public int Width { get; }
        public int Height { get; }
        public int Size { get; }

        public bool this[int x, int y]
        {
            get => innerArray.IsSet(x * Height + y);
            set => innerArray.Set(x * Height + y, value);
        }

        public void Dispose()
        {
            innerArray.Dispose();
        }
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

            NativeBitMatrix matrix = new(nodeCount, nodeCount, Allocator.Persistent);
            NativeList<Path> paths = new(nodeCount * nodeCount, Allocator.Persistent);
            NativeQueue<Path> actionQueue = new(Allocator.Persistent);
            NativeArray<bool> anyChangesArr = new(1, Allocator.Persistent);

            foreach(var edge in graph.Edges)
            {
                var min = math.min(edge.Start.ID, edge.End.ID);
                var max = math.max(edge.Start.ID, edge.End.ID);

                matrix[min, max] = true;

                var p = new Path
                (
                    (1UL << min) | (1UL << max),
                    1,
                    min,
                    max
                );

                paths.Add(p);
            }
            
            var core = new PathFinderCoreJob
            {
                matrix = matrix,
                input = paths.AsParallelReader(),
                output = actionQueue.AsParallelWriter(),
                anyChanges = anyChangesArr
            };
            var merge = new PathFinderMergeJob
            {
                input = actionQueue,
                output = paths
            };

            var anyChanges = true;

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
                anyChangesArr[0] = false;

                var handle = core.Schedule(paths.Length, 16);
                handle.Complete();

                anyChanges = anyChangesArr[0];

                handle = merge.Schedule();
                handle.Complete();
                
                // Debug.Log("Did it once, hyay!");
                // Debug.Log(anyChanges);

                // var s = "";

                // for (int i = 0; i < paths.Length; i++)
                // {
                //     var p = paths[i];

                //     s += $"{p}";
                //     s += "\n";
                // }

                // Debug.Log(s);

                count++;

                if(count > 100) break;
            }

            var pathlist = paths.ToArray();

            matrix.Dispose();
            paths.Dispose();
            actionQueue.Dispose();

            return pathlist;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct PathFinderMergeJob : IJob 
    {
        public NativeQueue<Path> input;
        public NativeList<Path> output;

        public void Execute()
        {
            output.Clear();

            while(input.TryDequeue(out var path))
            {
                var shouldcontinue = false;

                for(int i = 0; i < output.Length; i++)
                {
                    var other = output[i];
                    if(other.Mask == path.Mask && (other.End == path.Start | other.End == path.End)) 
                    {
                        shouldcontinue = true;
                        break;
                    }
                }

                if(shouldcontinue) continue;
                
                output.Add(path);

                // Debug.Log($"{action.Connection.Min} -> {action.Connection.Max}, {action.Edge}");
            }

            input.Clear();
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct PathFinderCoreJob : IJobParallelFor
    {
        [ReadOnly] public NativeBitMatrix matrix;

        [ReadOnly] public NativeArray<Path>.ReadOnly input;
        [WriteOnly] public NativeQueue<Path>.ParallelWriter output;
        [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<bool> anyChanges;

        public void Execute(int i)
        {
            Path p = input[i];

            var pcur = p.End;
            var pother = p.Start;

            var shouldAppendThis = true;
    
            for(int other = 0; other < matrix.Width; other++)
            {
                if(other == pcur | other == pother) continue;

                var min = math.min(pcur, other);
                var max = math.max(pcur, other);

                bool b = matrix[min, max];
                if (!b) break;
                
                var othermask = 1UL << other;
                var newmask = p.Mask | othermask;

                if(newmask != p.Mask) 
                {
                    shouldAppendThis = false;
                    output.Enqueue(new(newmask, p.Length + 1, min, max));
                }
            }

            if(shouldAppendThis)
            {
                output.Enqueue(p);
            }
            
            if(!shouldAppendThis) 
            {
                anyChanges[0] = true;
            }
        }
    }
}