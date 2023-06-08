using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Profiling;

namespace Ramsey.Graph.Experimental
{
    [BurstCompile(CompileSynchronously = true)]
    internal struct PathAggregateJob : IJob
    {
        public NativeQueue<JobPathGeneration> input;
        public NativeHashSet<JobPathInternal> deadOutput;
        public NativeList<JobPathInternal> liveOutput;
        public NativeHashSet<JobPathInternal> liveSet;

        public void Execute()
        {
            liveOutput.Clear();
            liveSet.Clear();

            while(input.TryDequeue(out var gen))
            {
                if(deadOutput.Contains(gen.Path)) continue;

                if(gen.IsLive)
                {
                    if(liveSet.Add(gen.Path)) liveOutput.Add(gen.Path);
                }
                else 
                {
                    deadOutput.Add(gen.Path);
                }
            }
        }
    }
}