using Unity.Collections;
using System;

namespace Ramsey.Graph.Experimental
{
    public static class AdjacencyMatrixUtils
    {
        public static NativeBitMatrix ToNative(this IReadOnlyAdjacencyMatrix mat, Allocator alloc)
        {
            var n = new NativeBitMatrix(mat.Size, mat.Size, alloc);

            for(int i = 0; i < mat.Size; i++)
            {
                for(int j = 0; j < mat.Size; j++)
                {
                    n[i, j] = mat.AreAdjacent(i, j);
                }
            }

            return n;
        }
    }

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