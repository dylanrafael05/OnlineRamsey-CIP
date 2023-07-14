using Unity.Collections;
using System;

namespace Ramsey.Graph.Experimental
{
    /// <summary>
    /// Wraps a value in a way such that it may be
    /// written to and read from a field in a job struct,
    /// even outside the scope of the job.
    /// </summary>
    public struct NativeValue<T> : IDisposable 
        where T : unmanaged
    {
        public NativeValue(Allocator alloc, T start = default)
        {
            arr = new(1, alloc);
            arr[0] = start;
        }

        [NativeDisableParallelForRestriction] private NativeArray<T> arr;
        public T Value 
        {
            get => arr[0];
            set => arr[0] = value;
        }

        public void Dispose()
        {
            arr.Dispose();
        }
    }
}