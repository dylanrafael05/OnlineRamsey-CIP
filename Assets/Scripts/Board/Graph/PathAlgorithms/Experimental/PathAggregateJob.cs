using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

namespace Ramsey.Graph.Experimental
{
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
}