using System;
using Unity.Collections;

namespace Ramsey.Graph.Experimental
{
    public readonly struct ValueBlock<T>
    {
        private readonly (T value, bool hasValue)[] values;

        public int Capacity => values.Length;

        public ValueBlock(int capacity)
        {
            values = new (T, bool)[capacity];
        }

        public Cell GetCell(int index) 
        {
            #if DEBUG
            if(index < 0 || index >= Capacity) 
                throw new IndexOutOfRangeException($"Index into cell block must be in range 0 and {Capacity}");
            #endif

            return new Cell(values, index);
        }

        public readonly struct Cell
        {
            public static Cell Null => default;

            internal Cell((T, bool)[] values, int index) 
            {
                this.values = values;
                this.index = index;
            }

            public bool HasValue => values[index].hasValue;

            public bool IsNull => values == null;

            private readonly (T value, bool hasValue)[] values;
            private readonly int index;
            public T Value
            {
                readonly get => values[index].value;
                set {values[index] = (value, true);}
            }
        }
    }
}