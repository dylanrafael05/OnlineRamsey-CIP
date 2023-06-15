using System;

namespace Ramsey.Graph
{
    /// <summary>
    /// Thrown when a graph's node count exceeds its maximum.
    /// </summary>
    public class GraphTooComplexException : Exception
    {
        public GraphTooComplexException(int count, string message = null) : base(message ?? $"Graph cannot exceed {count} nodes!")
        {
            Count = count;
        }

        public int Count { get; }
    }
}