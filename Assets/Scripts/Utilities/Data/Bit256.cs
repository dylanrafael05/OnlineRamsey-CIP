using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Mathematics;

namespace Ramsey.Utilities
{
    /// <summary>
    /// Represents a set of 256 bits.
    /// 
    /// This struct supports simple bitwise operations
    /// like &amp;, |, ^, &gt;&gt;, &lt;&lt;, and ~, alongside basics like == 
    /// and !=.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Bit256
    {
        private Bit256(ulong l1, ulong l2, ulong l3, ulong l4)
        {
            this.l1 = l1;
            this.l2 = l2;
            this.l3 = l3;
            this.l4 = l4;
        }
        
        public static Bit256 Zero => new(0, 0, 0, 0);
        public static Bit256 One => new(0, 0, 0, 1);
        public static Bit256 All => new(~0ul, ~0ul, ~0ul, ~0ul);

        [FieldOffset(0)]  private readonly ulong l1; 
        [FieldOffset(8)]  private readonly ulong l2; 
        [FieldOffset(16)] private readonly ulong l3;
        [FieldOffset(24)] private readonly ulong l4;

        public static Bit256 operator &(Bit256 a, Bit256 b) 
            => new(a.l1 & b.l1, a.l2 & b.l2, a.l3 & b.l3, a.l4 & b.l4);
        public static Bit256 operator |(Bit256 a, Bit256 b) 
            => new(a.l1 | b.l1, a.l2 | b.l2, a.l3 | b.l3, a.l4 | b.l4);
        public static Bit256 operator ^(Bit256 a, Bit256 b) 
            => new(a.l1 ^ b.l1, a.l2 ^ b.l2, a.l3 ^ b.l3, a.l4 ^ b.l4);
        public static Bit256 operator ~(Bit256 a) 
            => new(~a.l1, ~a.l2, ~a.l3, ~a.l4);

        public static Bit256 operator <<(Bit256 a, int b) 
        {
            if (b >= 256) return Zero;

            while (b >= 64)
            {
                a = Shl64(a);
                b -= 64;
            }

            ulong l4 = 0;

            l4 |= BitOps.ShlCarry(a.l4, b, out var l3);
            l3 |= BitOps.ShlCarry(a.l3, b, out var l2);
            l2 |= BitOps.ShlCarry(a.l2, b, out var l1);
            l1 |= a.l1 << b;

            return new(l1, l2, l3, l4);
        }

        public static Bit256 operator >>(Bit256 a, int b) 
        {
            if (b >= 256) return Zero;

            while (b >= 64)
            {
                a = Shr64(a);
                b -= 64;
            }

            ulong l1 = 0;

            l1 |= BitOps.ShrCarry(a.l1, b, out var l2);
            l2 |= BitOps.ShrCarry(a.l2, b, out var l3);
            l3 |= BitOps.ShrCarry(a.l3, b, out var l4);
            l4 |= a.l4 >> b;

            return new(l1, l2, l3, l4);
        }

        public static bool operator ==(Bit256 a, Bit256 b) 
            => (a.l1, a.l2, a.l3, a.l4) == (b.l1, b.l2, b.l3, b.l4);
        public static bool operator !=(Bit256 a, Bit256 b) 
            => (a.l1, a.l2, a.l3, a.l4) != (b.l1, b.l2, b.l3, b.l4);

        public override bool Equals(object obj)
            => obj is Bit256 b && this == b;
        public override int GetHashCode()
            => l1.GetHashCode() ^ l2.GetHashCode() ^ l3.GetHashCode() ^ l4.GetHashCode();
        public override string ToString()
            => Convert.ToString((long)l1, 2).PadLeft(64, '0') 
            + Convert.ToString((long)l2, 2).PadLeft(64, '0') 
            + Convert.ToString((long)l3, 2).PadLeft(64, '0') 
            + Convert.ToString((long)l4, 2).PadLeft(64, '0') ;

        /// <summary>
        /// Shift the data within this structure left by 64 bits.
        /// </summary>
        public static Bit256 Shl64(Bit256 a)
            => new(a.l2, a.l3, a.l4, 0);
        /// <summary>
        /// Shift the data within this structure right by 64 bits.
        /// </summary>            
        public static Bit256 Shr64(Bit256 a)
            => new(0, a.l1, a.l2, a.l3);

        /// <summary>
        /// Get the number of bits within this structure that are set.
        /// </summary>
        public static int Bitcount(Bit256 a) 
            => math.countbits(a.l1) + math.countbits(a.l2) + math.countbits(a.l3) + math.countbits(a.l4);

        public static explicit operator long(Bit256 b) 
            => (long)b.l4;
        public static implicit operator Bit256(ulong a)
            => new(0, 0, 0, a);
        public static explicit operator Bit256(long a)
            => new(0, 0, 0, (ulong)a);
        public static implicit operator Bit256(uint a)
            => new(0, 0, 0, a);
        public static explicit operator Bit256(int a)
            => new(0, 0, 0, (ulong)a);

        /// <summary>
        /// Get an iterator over the set bit positions in the
        /// given Bit256.
        /// </summary>
        public static BitposIterator IterateBitpos(Bit256 b) 
            => new(b);
        /// <summary>
        /// Get the first bit position set within the given
        /// Bit256.
        /// </summary>
        public static int FirstBitpos(Bit256 b) 
        {
            IterateBitpos(b).GetNext(out var x);
            return x;
        }

        /// <summary>
        /// Iterates over the bits within a Bit256,
        /// returning their positions.
        /// </summary>
        public struct BitposIterator
        {
            private Bit256 bits;
            private int currentIndex;

            internal BitposIterator(Bit256 bits) 
            {
                this.bits = bits;
                currentIndex = 0;
            }

            /// <summary>
            /// Get the next bit position from the Bit256 this operates on,
            /// returned through <paramref name="pos"/>. Returns true if there was
            /// a next position, and false if the end of the set bits has been reached.
            /// </summary>
            public bool GetNext(out int pos)
            {
                pos = -1;

                while((bits.l4 & 1) == 0)
                {
                    bits >>= 1;
                    currentIndex++;

                    if(bits == 0) return false;
                }

                pos = currentIndex;

                bits >>= 1;
                currentIndex++;

                return true;
            }
        }
    }
}