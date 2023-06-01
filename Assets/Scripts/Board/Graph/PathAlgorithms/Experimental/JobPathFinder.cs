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
using Ramsey.Utilities;

namespace Ramsey.Graph.Experimental
{
    public class JobsPathFinder : IIncrementalPathFinder
    {
        private List<List<IPath>> paths = new();
        private List<IPath> maxPaths = new();

        public IEnumerable<IPath> AllPaths => paths.Merge();
        public IReadOnlyList<IPath> MaxPathsByType => maxPaths;

        public void HandleNodeAddition(Node node)
        {}

        private void EnsurePathTypeAvailable(int type)
        {
            while(maxPaths.Count <= type)
            {
                maxPaths.Add(null);
            }

            while(paths.Count <= type)
            {
                paths.Add(new());
            }
        }

        public async Task HandlePaintedEdge(Edge edge, Graph graph)
        {
            // Prepare type
            var type = edge.Type;
            EnsurePathTypeAvailable(type);

            paths[type].Clear();
            maxPaths[type] = null;

            // Find all
            var pathsInternal = await Task.Run(() => JobPathFinderImpl.FindAll(graph, type));

            // Get maximum path
            foreach(var p in pathsInternal) 
            {
                var jp = new JobPath(p, graph, type);

                if(maxPaths[type] == null || p.Length > maxPaths[type].Length)
                {
                    maxPaths[type] = jp;
                }

                paths[type].Add(jp);
            }
        }
    }

    public class JobPath : IPath
    {
        internal JobPath(JobPathInternal from, Graph graph, int type)
        {
            mask = from.Mask;

            // Debug.Log(from);

            Start = graph.NodeFromID(from.Start);
            End = graph.NodeFromID(from.End);

            Length = from.Length;

            IEnumerable<List<Node>> FindSequence(List<Node> nodesSoFar, List<Node> nodesRemaining)
            {
                if(nodesRemaining.Count == 0)
                {
                    if(nodesSoFar.Last().ID == from.End)
                    {
                        yield return nodesSoFar;
                    }

                    yield break;
                }

                foreach(var node in nodesRemaining)
                {
                    var edgeToNext = nodesSoFar.Last().EdgeConnectedTo(node);
                    if(edgeToNext != null && edgeToNext.Type == type)
                    {
                        foreach(var seq in FindSequence(nodesSoFar.Append(node).ToList(), nodesRemaining.Where(n => n != node).ToList()))
                        {
                            yield return seq;
                        }
                    }
                }
            }

            var allNodes = MathUtils.BitPositions(mask ^ (1UL << from.Start)).Select(graph.NodeFromID).ToList();
            nodes = new(() => FindSequence(new() {Start}, allNodes).First());

            Type = type;
        }

        private readonly ulong mask;
        private readonly Lazy<List<Node>> nodes;

        public int Type { get; }
        public IEnumerable<Node> Nodes => nodes.Value;

        public Node Start { get; }
        public Node End { get; }
        public int Length { get; }

        public IEnumerable<Edge> Edges
            => PathUtils.GetEdgesConnecting(nodes.Value);

        public bool Contains(Node node)
            => (mask | (1UL << node.ID)) == mask;

        public bool IsEndpoint(Node node)
            => node == Start || node == End;
    }

    internal readonly struct JobPathInternal 
    {
        public JobPathInternal(ulong mask, int count, int start, int end) 
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

    public struct NativeReturn<T> : IDisposable 
        where T : unmanaged
    {
        public NativeReturn(Allocator alloc, T start = default)
        {
            arr = new(1, alloc);
            arr[0] = start;
        }

        [NativeDisableParallelForRestriction] private NativeArray<T> arr;
        public T Value 
        {
            get => arr[0];
            set => arr[0] = value;
        }

        public void Dispose()
        {
            arr.Dispose();
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

    internal static class JobPathFinderImpl
    {
        internal static JobPathInternal[] FindAll(Graph graph, int type) 
        {
            var nodeCount = graph.Nodes.Count;

            NativeBitMatrix matrix = new(nodeCount, nodeCount, Allocator.Persistent);
            NativeList<JobPathInternal> deadPaths = new(nodeCount * nodeCount, Allocator.Persistent);
            NativeList<JobPathInternal> livePaths = new(nodeCount * nodeCount, Allocator.Persistent);
            NativeQueue<JobPathInternal> actionQueue = new(Allocator.Persistent);
            NativeReturn<bool> anyChangesWrap = new(Allocator.Persistent);

            foreach(var edge in graph.Edges.Where(e => e.Type == type))
            {
                var min = math.min(edge.Start.ID, edge.End.ID);
                var max = math.max(edge.Start.ID, edge.End.ID);

                matrix[min, max] = true;

                var p = new JobPathInternal
                (
                    (1UL << min) | (1UL << max),
                    1,
                    min,
                    max
                );

                livePaths.Add(p);
            }

            // var s = "";
            // for(int j = 0; j < matrix.Height; j++)
            // {
            //     s += j + ": ";
            //     for(int i = 0; i < matrix.Width; i++)
            //     {
            //         s += matrix[i, j] ? '#' : '-';
            //     }
            //     s += "\n";
            // }
            // Debug.Log(s);
            
            var core = new PathGenerateJob
            {
                matrix = matrix,
                input = livePaths.AsParallelReader(),
                output = actionQueue.AsParallelWriter(),
                anyChanges = anyChangesWrap
            };
            var merge = new PathAggregateJob
            {
                input = actionQueue,
                deadOutput = deadPaths,
                liveOutput = livePaths
            };

            var anyChanges = true;
            var step = 1;

            while(anyChanges)
            {
                anyChangesWrap.Value = false;

                core.output = actionQueue.AsParallelWriter();
                core.input = livePaths.AsParallelReader();
                core.matrix = matrix;

                core.step = step;

                var handle = core.Schedule(livePaths.Length, 16);
                handle.Complete();

                anyChanges = anyChangesWrap.Value;

                merge.step = step;

                handle = merge.Schedule();
                handle.Complete();

                step++;

                if(step > 100) break;
            }

            var pathlist = deadPaths.ToArray();

            matrix.Dispose();
            deadPaths.Dispose();
            livePaths.Dispose();
            actionQueue.Dispose();
            anyChangesWrap.Dispose();

            return pathlist;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    internal struct PathAggregateJob : IJob 
    {
        public int step;
        public NativeQueue<JobPathInternal> input;
        public NativeList<JobPathInternal> deadOutput;
        public NativeList<JobPathInternal> liveOutput;

        public void Execute()
        {
            liveOutput.Clear();

            while(input.TryDequeue(out var path))
            {
                var shouldcontinue = false;

                // Duplicates in all
                for(int i = 0; i < deadOutput.Length; i++)
                {
                    var other = deadOutput[i];
                    if((other.Mask == path.Mask) & ((other.End == path.Start) | (other.End == path.End))) 
                    {
                        shouldcontinue = true;
                        break;
                    }
                }

                if(shouldcontinue) continue;

                // Duplicates in live
                for(int i = 0; i < liveOutput.Length; i++)
                {
                    var other = liveOutput[i];
                    if((other.Mask == path.Mask) & ((other.End == path.Start) | (other.End == path.End))) 
                    {
                        shouldcontinue = true;
                        break;
                    }
                }

                if(shouldcontinue) continue;
                
                if(path.Length <= step)
                {
                    deadOutput.Add(path);
                }
                else 
                {
                    liveOutput.Add(path);
                }

                // Debug.Log($"{action.Connection.Min} -> {action.Connection.Max}, {action.Edge}");
            }

            input.Clear();
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    internal struct PathGenerateJob : IJobParallelFor
    {
        [ReadOnly, NativeDisableParallelForRestriction] public NativeBitMatrix matrix;
        [ReadOnly] public int step;

        [ReadOnly, NativeMatchesParallelForLength] public NativeArray<JobPathInternal>.ReadOnly input;
        [WriteOnly] public NativeQueue<JobPathInternal>.ParallelWriter output;
        [WriteOnly, NativeDisableParallelForRestriction] public NativeReturn<bool> anyChanges;

        public void Execute(int i)
        {
            JobPathInternal p = input[i];

            if(p.Length < step) return;

            var pcur = p.End;
            var pother = p.Start;

            var newChangesPresent = false;

            var w = matrix.Width;
    
            for(int other = 0; other < w; other++)
            {
                if(other == pcur || other == pother) continue;

                var min = math.min(pcur, other);
                var max = math.max(pcur, other);

                // Debug.Log($"Trying {min} -> {max}");
                bool b = matrix[min, max];
                if (!b) continue;

                // Debug.Log($"Checking {min} -> {max}");
                
                var othermask = 1UL << other;
                var newmask = p.Mask | othermask;

                if(newmask != p.Mask) 
                {
                    newChangesPresent = true;
                    output.Enqueue(new(newmask, p.Length + 1, pother, other));
                }
            }

            pcur = p.Start;
            pother = p.End;

            for(int other = 0; other < w; other++)
            {
                if(other == pcur || other == pother) continue;

                var min = math.min(pcur, other);
                var max = math.max(pcur, other);

                // Debug.Log($"Trying {min} -> {max}");
                bool b = matrix[min, max];
                if (!b) continue;
                
                // Debug.Log($"Checking {min} -> {max}");
                
                var othermask = 1UL << other;
                var newmask = p.Mask | othermask;

                if(newmask != p.Mask) 
                {
                    newChangesPresent = true;
                    output.Enqueue(new(newmask, p.Length + 1, pother, other));
                }
            }
            
            if(!newChangesPresent)
            {
                output.Enqueue(p);
            }
            else
            {
                anyChanges.Value = true;
            }
        }
    }
}