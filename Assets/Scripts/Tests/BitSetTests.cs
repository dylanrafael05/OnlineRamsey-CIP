using System.Linq;
using NUnit.Framework;
using Ramsey.Utilities;

[TestFixture, TestOf(typeof(BitSet))]
public class BitSetTests
{
    [Test]
    public void BitSetEmptyIsEmpty()
    {
        var bs = new BitSet();

        Assert.IsEmpty(bs);
    }

    public static readonly object[] Indices = {0, 1, 5, 20, 40, 80, 150, 300};

    [Test, TestCaseSource(nameof(Indices))]
    public void BitSetSizeIsCorrect(int index)
    {
        var bs = new BitSet();
        bs.Set(index);

        Assert.AreEqual(bs.Count, index + 1);
    }

    [Test, TestCaseSource(nameof(Indices))]
    public void BitSetGetSetWorks(int index) 
    {
        var bs = new BitSet();
        bs.Set(index);

        Assert.IsTrue(bs.IsSet(index));
        Assert.IsFalse(bs.IsUnset(index));
    }

    [Test, TestCaseSource(nameof(Indices))]
    public void BitSetUnsetWorks(int index) 
    {
        var bs = new BitSet();
        bs.Set(index);
        bs.Unset(index);

        Assert.IsFalse(bs.IsSet(index));
        Assert.IsTrue(bs.IsUnset(index));
    }

    [Test, TestCaseSource(nameof(Indices))]
    public void BitSetFlipWorks(int index) 
    {
        var bs = new BitSet();
        bs.Set(index);

        bs.Flip(index);
        Assert.IsFalse(bs.IsSet(index));
        
        bs.Flip(index);
        Assert.IsTrue(bs.IsSet(index));
    }
    
    public static readonly object[] Sequences = {
        new[] {false, true},
        new[] {true, true, false, false},
        new[] {true, true, false, true},
        new[] {
            true, true, false, true, 
            true, true, false, true, 
            true, true, false, true, 
            true, true, false, true, 
            true, true, false, true, 
            true, true, false, true, 
            true, true, false, true, 
            true, true, false, true, 
            true, true, false, true, 
            true, true, false, true, 
            true, true, false, true, 
            true, true, false, true, 
        }
    };

    [Test, TestCaseSource(nameof(Sequences))]
    public void BitSetIterationWorks(bool[] expected)
    {
        var bs = new BitSet(expected.Length);

        for(var i = 0; i < expected.Length; i++)
        {
            if(expected[i]) bs.Set(i);
        }

        Assert.AreEqual(expected.Length, bs.Count);
        Assert.That(bs.SequenceEqual(expected), string.Join(", ", bs) + " should have been " + string.Join(", ", expected));
    }

    [Test, TestCaseSource(nameof(Sequences))]
    public void ToBitSetWorks(bool[] expected)
    {
        var bs = expected.ToBitSet();

        Assert.AreEqual(expected.Length, bs.Count);
        Assert.That(bs.SequenceEqual(expected), string.Join(", ", bs) + " should have been " + string.Join(", ", expected));
    }
}
