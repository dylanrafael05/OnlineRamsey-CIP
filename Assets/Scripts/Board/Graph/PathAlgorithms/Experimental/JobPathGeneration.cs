using Ramsey.Utilities;

namespace Ramsey.Graph.Experimental
{
    internal readonly struct JobPathGeneration
    {
        public JobPathGeneration(JobPathInternal path, bool isLive)
        {
            Path = path;
            IsLive = isLive;
        }

        public JobPathGeneration(Bit256 mask, int count, int start, int end, bool isLive) 
            : this(new(mask, count, start, end), isLive)
        {}

        public JobPathInternal Path { get; }
        public bool IsLive { get; }
    }
}