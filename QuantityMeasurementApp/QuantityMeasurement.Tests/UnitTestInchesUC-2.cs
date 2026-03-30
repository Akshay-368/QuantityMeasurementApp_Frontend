using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Units;
using QuantityMeasurement.ModelLayerLayer.Core;

namespace QuantityMeasurement.Tests;

[TestFixture]
public class InchesTests
{
    [Test]
    public void Given1InchAnd1Inch_WhenCompared_ShouldReturnTrue()
    {
        var i1 = new Inches(1.0);
        var i2 = new Inches(1.0);
        Assert.That(i1.Equals(i2), Is.True);
    }

    /// <summary>
    /// UC2 Requirement: Handles Reflexive, Null, Type, and Value equality.
    /// In this test, we are comparing two different Inch objects with different values.
    /// The result should be false.
    /// </summary>
    [Test]
    public void Given1InchAnd2Inches_WhenCompared_ShouldReturnFalse()
    {
        var i1 = new Inches(1.0);
        var i2 = new Inches(2.0);
        Assert.That(i1.Equals(i2), Is.False);
    }
    
    // UC2 Challenge: Comparing Feet to Inches
    [Test]
    public void Given1FeetAnd1Inch_WhenCompared_ShouldReturnFalse()
    {
        var f1 = new Feet(1.0);
        var i1 = new Inches(1.0);
        
        // This hits your 'Type Check' in the AbstractBase!
        Assert.That(f1.Equals(i1), Is.False);
    }
}