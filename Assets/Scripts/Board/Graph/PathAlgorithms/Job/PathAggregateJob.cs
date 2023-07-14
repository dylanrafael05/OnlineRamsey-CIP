using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Profiling;

namespace Ramsey.Graph.Experimental
{
    /// <summary>
    /// The job which handles the outputs of the generation job,
    /// reoving duplicates and storing them into their appropriate 
    /// places.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    internal struct PathAggregateJob : IJob
    {
        public NativeQueue<JobPathGeneration> input;
        public NativeHashSet<JobPathInternal> deadOutput;
        public NativeList<JobPathInternal> liveOutput;
        public NativeHashSet<JobPathInternal> liveSet;
        public NativeValue<JobPathInternal> longest;

        public void Execute()
        {
            liveOutput.Clear();
            liveSet.Clear();

            while(input.TryDequeue(out var gen))
            {
                if(deadOutput.Contains(gen.Path)) continue;

                if(gen.IsLive)
                {
                    if(liveSet.Add(gen.Path)) 
                    {
                        liveOutput.Add(gen.Path);
                    }
                }
                else 
                {
                    if(longest.Value.Length < gen.Path.Length) longest.Value = gen.Path;
                    deadOutput.Add(gen.Path);
                }
            }
        }
    }
}