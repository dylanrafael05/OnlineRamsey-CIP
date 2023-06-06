using System;

namespace Ramsey.Graph
{
    /// <summary>
    /// Thrown when a graph's node count exceeds its maximum.
    /// </summary>
    public class GraphTooComplexException : Exception
    {
        public GraphTooComplexException(int count) : base($"Graph cannot exceed {count} nodes!")
        {
            Count = count;
        }

        public int Count { get; }
    }
}