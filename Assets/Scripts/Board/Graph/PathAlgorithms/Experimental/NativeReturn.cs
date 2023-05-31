using Unity.Collections;
using System;

namespace Ramsey.Graph.Experimental
{
    public struct NativeReturn<T> : IDisposable 
        where T : unmanaged
    {
        public NativeReturn(Allocator alloc, T start = default)
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