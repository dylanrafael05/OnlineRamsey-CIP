using System;
using Ramsey.Utilities;

namespace Ramsey.Graph.Experimental
{
    internal readonly struct JobPathInternal : IEquatable<JobPathInternal>
    {
        public JobPathInternal(Bit256 mask, byte start, byte end) 
        {
            Mask = mask;
            Start = start;
            End = end;
        }

        public Bit256 Mask { get; }
        public byte Start { get; }
        public byte End { get; }

        public int Length => Bit256.Bitcount(Mask) - 1;

        public bool Equals(JobPathInternal other) 
            => (Mask == other.Mask) & ((Start == other.Start) | (Start == other.End)) & ((End == other.Start) | (End == other.End));

        public override int GetHashCode()
        {
            return Mask.GetHashCode() ^ (Start * 899) ^ (End * 567);
        }

        public override string ToString()
        {
            return $"{Start} -> {End} ; length {Length} [{Convert.ToString((long)Mask, 2).PadLeft(32, '-')}]";
        }
    }
}