using Unity.Collections;
using System;

namespace Ramsey.Graph.Experimental
{
    public readonly struct NativeBitMatrix : IDisposable 
    {
        public NativeBitMatrix(int w, int h, Allocator alloc) 
        {
            Width = w;
            Height = h;
            Size = w * h;

            innerArray = new(Size, alloc);
        }

        private readonly NativeBitArray innerArray;
        public int Width { get; }
        public int Height { get; }
        public int Size { get; }

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