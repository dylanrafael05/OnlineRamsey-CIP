using System;
using Ramsey.Utilities;

namespace Ramsey.Graph.Experimental
{
    /// <summary>
    /// Represents a path constructed using the jobs approach.
    /// <br/>
    /// 
    /// The data stored within this structure is inadequate to 
    /// distinguish any two arbitrary paths, since no true ordering
    /// of nodes is present other than the knowledge of which is first
    /// and which is last. This does not pose a problem however, since
    /// the inclusion of a node anywhere within a path disqualifies it
    /// from being included again within an expansion of the path, meaning
    /// that true ordering is unnecessary to the path finder algorithm.
    /// <br/>
    /// 
    /// By taking advantage of this fact, this structure requires no allocations
    /// and has a constant size, making it much easier and faster to work with.
    /// <br/>
    /// 
    /// However, this does mean that the final results of the algorithm
    /// must be post-processed. <see cref="JobPath"/>
    /// </summary>
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