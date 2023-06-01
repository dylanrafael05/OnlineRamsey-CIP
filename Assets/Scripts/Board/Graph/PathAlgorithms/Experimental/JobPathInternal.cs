using System;

namespace Ramsey.Graph.Experimental
{
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
}