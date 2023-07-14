using Unity.Collections;
using System;

namespace Ramsey.Graph.Experimental
{
    /// <summary>
    /// Stores a native matrix of bits.
    /// </summary>
    public readonly struct NativeBitMatrix : IDisposable 
    {
        public NativeBitMatrix(int w, int h, Allocator alloc) 
        {
            Width = w;
            Height = h;
            Area = w * h;

            innerArray = new(Area, alloc);
        }

        private readonly NativeBitArray innerArray;
        public int Width { get; }
        public int Height { get; }
        public int Area { get; }

        public bool this[int x, int y]
        {
            get => innerArray.IsSet(x * Height + y);
            set => innerArray.Set(x * Height + y, value);
        }

        public void Dispose()
        {
            innerArray.Dispose();
        }
    }
}