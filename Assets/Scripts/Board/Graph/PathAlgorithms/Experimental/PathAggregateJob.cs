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

        public void Execute()
        {
            liveOutput.Clear();

            while(input.TryDequeue(out var gen))
            {
                var path = gen.Path;

                // Duplicates in dead
                var shouldcontinue = deadOutput.Contains(gen.Path);
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
                
                // Sort in
                if(gen.IsLive)
                {
                    liveOutput.Add(path);
                }
                else 
                {
                    deadOutput.Add(path);
                }
            }
        }
    }
}