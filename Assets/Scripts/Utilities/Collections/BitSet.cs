using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ramsey.Utilities
{
    /// <summary>
    /// Represents a set of booleans as a list of bits.
    /// </summary>
    public class BitSet : IEnumerable<bool>
    {
        private int count;
        private uint[] values;

        /// <summary>
        /// The number of bits which can be stored in one
        /// "element" of this set.
        /// </summary>
        public const int ElementSize = sizeof(uint) * 8;

        /// <summary>
        /// The raw elements which make up this set.
        /// </summary>
        public IReadOnlyList<uint> Elements => values;

        /// <summary>
        /// The number of elements within this set.
        /// </summary>
        public int Count => count;
        /// <summary>
        /// The number of elements that this set can 
        /// store without needing to reallocate itself.
        /// </summary>
        public int Capacity => values.Length * ElementSize;

        /// <summary>
        /// Create an empty bitset.
        /// </summary>
        public BitSet()
        {
            count = 0;
            values = new uint[0];
        }

        /// <summary>
        /// Create a copy of the given bitset.
        /// </summary>
        public BitSet(BitSet other) 
        {
            count = other.count;
            values = other.values.ToArray();
        }

        /// <inheritdoc cref="BitSet(BitSet)"/>
        public BitSet Clone() 
            => new(this);

        private void HandleIndex(int index, out int valueIndex, out uint mask) 
        {   
            // INVARIANTS //
            if(index < 0) throw new IndexOutOfRangeException();

            // CALCULATIONS //
            valueIndex   = index / ElementSize;
            var subIndex = index % ElementSize;

            mask = 1u << subIndex;
        }

        private void HandleIndexMutable(int index, out int valueIndex, out uint mask) 
        {
            // INVARIANTS //
            if(index < 0) throw new IndexOutOfRangeException();
            
            // ENSURE CAPACITY //
            EnsureCount(index + 1);
            
            // HANDLE INDEX //
            HandleIndex(index, out valueIndex, out mask);
        }

        /// <summary>
        /// Set the bit at the given index to 'true'
        /// </summary>
        public void Set(int index)
        {
            HandleIndexMutable(index, out var valueIndex, out var mask);

            values[valueIndex] |= mask;
        }
        /// <summary>
        /// Set the bit at the given index to 'false'
        /// </summary>
        public void Unset(int index)
        {
            HandleIndexMutable(index, out var valueIndex, out var mask);

            values[valueIndex] &= ~mask;
        }
        /// <summary>
        /// Flip the bit at the given index
        /// </summary>
        public void Flip(int index)
        {
            HandleIndexMutable(index, out var valueIndex, out var mask);

            values[valueIndex] ^= mask;
        }

        /// <summary>
        /// Check if the bit at the given index is set to 'true'
        /// </summary>
        public bool IsSet(int index) 
        {
            if(index > Count) return false;

            HandleIndex(index, out var valueIndex, out var mask);

            return (values[valueIndex] & mask) != 0;
        }
        /// <summary>
        /// Check if the bit at the given index is set to 'false'
        /// </summary>
        public bool IsUnset(int index) 
        {
            if(index > Count) return true;

            HandleIndexMutable(index, out var valueIndex, out var mask);

            return (values[valueIndex] & mask) == 0;
        }

        /// <summary>
        /// Clear all the bits in this set.
        /// </summary>
        public void Clear()
        {
            for(var i = 0; i < values.Length; i++)
            {
                values[i] = 0;
            }
        }

        /// <summary>
        /// Ensure that the set can store the given number
        /// of elements.
        /// </summary>
        private void EnsureCount(int count)
        {
            if(count > this.count) this.count = count;

            if(count <= Capacity) return;

            // Add es-1 to account for integer rounding
            var neededCount = (count + ElementSize - 1) / ElementSize;

            var oldValues = values;
            values = new uint[neededCount];

            oldValues.CopyTo(values, 0);
        }

        public bool SetEquals(BitSet other)
        {
            return count == other.count && values.SequenceEqual(other.values);
        }

        public IEnumerator<bool> GetEnumerator()
        {
            var i = 0;
            var c = 0u;

            while(i < count)
            {
                if(i % ElementSize == 0)
                {
                    c = values[i / ElementSize];
                }
                else 
                {
                    c >>= 1;
                }

                yield return (c & 1) == 1;

                i++;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}