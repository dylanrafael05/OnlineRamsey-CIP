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

        public JobPathGeneration(Bit256 mask, byte start, byte end, bool isLive) 
            : this(new(mask, start, end), isLive)
        {}

        public JobPathInternal Path { get; }
        public bool IsLive { get; }
    }
}