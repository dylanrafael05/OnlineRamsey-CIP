using NUnit.Framework;
using Ramsey.Utilities;

[TestFixture, TestOf(typeof(Bit256))]
public class Bit256Tests
{
    public void Bit256ZeroWorks()
    {
        Assert.AreEqual(0, (int)Bit256.Zero);
    }

    public static readonly object[] Integers = {
        1, 2, 3, 4, 5,
        10, 20, 30, 40, 50,
        100, 200
    };
    public static readonly object[] IntegerPairs = {
        new object[] {1, 2},
        new object[] {5, 9},
        new object[] {712381, 9},
        new object[] {912831799, 123817}
    };

    [Test]
    public void Bit256IteratorWorks()
    {
        var bits = (Bit256.One << 3) | (Bit256.One << 1) | Bit256.One;
        var iter = Bit256.IterateBitpos(bits);

        int idx;

        Assert.True(iter.GetNext(out idx));
        Assert.AreEqual(0, idx);

        Assert.True(iter.GetNext(out idx));
        Assert.AreEqual(1, idx);

        Assert.True(iter.GetNext(out idx));
        Assert.AreEqual(3, idx);

        Assert.False(iter.GetNext(out idx));
        Assert.AreEqual(-1, idx);
    }
    
    [TestCaseSource(nameof(Integers))]
    public void Bit256ShlrWorks(int x)
    {
        Assert.AreEqual(0, (int)((Bit256.Zero << x) >> x));
    }

    [TestCaseSource(nameof(IntegerPairs))]
    public void Bit256AndWorks(int x, int y)
    {
        Assert.AreEqual(x & y, (int)((Bit256)x & (Bit256)y));
    }
    [TestCaseSource(nameof(IntegerPairs))]
    public void Bit256OrWorks(int x, int y)
    {
        Assert.AreEqual(x | y, (int)((Bit256)x | (Bit256)y));
    }
    [TestCaseSource(nameof(IntegerPairs))]
    public void Bit256XorWorks(int x, int y)
    {
        Assert.AreEqual(x ^ y, (int)((Bit256)x ^ (Bit256)y));
    }
}