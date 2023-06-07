using System;
using System.Runtime.InteropServices;
using Unity.Burst;

namespace Ramsey.Utilities
{
    [BurstCompile]
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
        [FieldOffset(32)] private readonly ulong l4;

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
            ulong l1, l2, l3, l4;
            
            if (b >= 256) return Zero;

            while (b >= 64)
            {
                a = Shl64(a);
                b -= 64;
            }

            l1 = (a.l1 << b) | (a.l2 << (64 - b));
            l2 = (a.l2 << b) | (a.l3 << (64 - b));
            l3 = (a.l3 << b) | (a.l4 << (64 - b));
            l4 = (a.l4 << b);

            return new(l1, l2, l3, l4);
        }

        public static Bit256 operator >>(Bit256 a, int b) 
        {
            ulong l1, l2, l3, l4;
            
            if (b >= 256) return Zero;

            while (b >= 64)
            {
                a = Shr64(a);
                b -= 64;
            }

            l1 = (a.l1 >> b);
            l2 = (a.l2 >> b) | (a.l1 >> (64 - b));
            l3 = (a.l3 >> b) | (a.l2 >> (64 - b));
            l4 = (a.l4 >> b) | (a.l3 >> (64 - b));

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

        public static Bit256 Shl64(Bit256 a)
            => new(a.l2, a.l3, a.l4, 0);
        public static Bit256 Shr64(Bit256 a)
            => new(0, a.l1, a.l2, a.l3);

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
    }
}