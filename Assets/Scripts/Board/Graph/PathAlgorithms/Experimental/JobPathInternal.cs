using System;
using Ramsey.Utilities;

namespace Ramsey.Graph.Experimental
{
    internal readonly struct JobPathInternal 
    {
        public JobPathInternal(Bit256 mask, int count, int start, int end) 
        {
            Mask = mask;
            Length = count;
            Start = start;
            End = end;
        }

        public int Length { get; }
        public Bit256 Mask { get; }
        public int Start { get; }
        public int End { get; }

        public override string ToString()
        {
            return $"{Start} -> {End} ; length {Length} [{Convert.ToString((long)Mask, 2).PadLeft(32, '-')}]";
        }
    }
}