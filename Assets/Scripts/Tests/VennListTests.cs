using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ramsey.Utilities;
using System.Linq;

[TestFixture, TestOf(typeof(VennList<>))]
public class VennListTests
{
    [Test]
    public void VennListEmptyIsEmpty()
    {
        var list = new VennList<object>();
        
        Assert.IsEmpty(list);
    }

    private static readonly object[] Sequences = 
    {
        new object[] {0},
        new object[] {" ", 0},
        new object[] {0.1, "hello", true},
        new object[] {null, new object()}
    };
    
    [Test, TestCaseSource(nameof(Sequences))]
    public void VennListAppendsProperly(object[] input)
    {
        var list = new VennList<object>(input);

        Assert.That(list.Append(null).SequenceEqual(input.Append(null)));
    }

    [Test, TestCaseSource(nameof(Sequences))]
    public void VennListPrependsProperly(object[] input)
    {
        var list = new VennList<object>(input);

        Assert.That(list.Prepend(null).SequenceEqual(input.Prepend(null)));
    }

    [Test, TestCaseSource(nameof(Sequences))]
    public void VennListCopyConstructorReportsCorrectly(object[] input)
    {
        var list = new VennList<object>(input);
        
        Assert.That(list.SequenceEqual(input));
    }
    
    [Test, TestCaseSource(nameof(Sequences))]
    public void VennListEnumerationReportsCorrectly(object[] input)
    {
        var list = new VennList<object>();

        foreach(var item in input) 
        {
            list = list.Append(item);
        }

        Assert.That(list.SequenceEqual(input));
    }

    [Test, TestCaseSource(nameof(Sequences))]
    public void VennListFirstReportsCorrectly(object[] input)
    {
        var list = new VennList<object>(input);

        Assert.AreEqual(list.First(), input.First());
    }

    [Test, TestCaseSource(nameof(Sequences))]
    public void VennListLastReportsCorrectly(object[] input)
    {
        var list = new VennList<object>(input);

        Assert.AreEqual(list.Last(), input.Last());
    }
}
