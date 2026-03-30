using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Units;
using QuantityMeasurement.ModelLayerLayer.Core;
using System;

namespace QuantityMeasurement.Tests;

[TestFixture]
public class LengthEqualityContractTests
{
    // -------------------------------------------------------
    // 1. Cross-Unit Equality
    // -------------------------------------------------------

    [Test]
    public void Given12InchesAnd1Feet_WhenCompared_ShouldReturnTrue()
    {
        Assert.That(new Inches(12).Equals(new Feet(1)), Is.True);
    }

    [Test]
    public void Given1FeetAnd12Inches_WhenCompared_ShouldReturnTrue()
    {
        Assert.That(new Feet(1).Equals(new Inches(12)), Is.True);
    }

    [Test]
    public void Given3FeetAnd1Yard_WhenCompared_ShouldReturnTrue()
    {
        Assert.That(new Feet(3).Equals(new Yard(1)), Is.True);
    }

    // -------------------------------------------------------
    // 2. Cross-Unit Inequality
    // -------------------------------------------------------

    [Test]
    public void Given1MeterAnd1Feet_WhenCompared_ShouldReturnFalse()
    {
        Assert.That(new Meter(1).Equals(new Feet(1)), Is.False);
    }

    [Test]
    public void Given10InchesAnd1Feet_WhenCompared_ShouldReturnFalse()
    {
        Assert.That(new Inches(10).Equals(new Feet(1)), Is.False);
    }

    // -------------------------------------------------------
    // 3. Symmetric Property
    // -------------------------------------------------------

    [Test]
    public void Equality_ShouldBeSymmetric()
    {
        var a = new Inches(12);
        var b = new Feet(1);

        Assert.That(a.Equals(b), Is.True);
        Assert.That(b.Equals(a), Is.True);
    }

    // -------------------------------------------------------
    // 4. Transitive Property
    // -------------------------------------------------------

    [Test]
    public void Equality_ShouldBeTransitive()
    {
        var a = new Feet(3);
        var b = new Yard(1);
        var c = new Inches(36);

        Assert.That(a.Equals(b), Is.True);
        Assert.That(b.Equals(c), Is.True);
        Assert.That(a.Equals(c), Is.True);
    }

    // -------------------------------------------------------
    // 5. Consistency
    // -------------------------------------------------------

    [Test]
    public void Equality_ShouldBeConsistent()
    {
        var a = new Feet(1);
        var b = new Inches(12);

        Assert.That(a.Equals(b), Is.True);
        Assert.That(a.Equals(b), Is.True);
        Assert.That(a.Equals(b), Is.True);
    }

    // -------------------------------------------------------
    // 6. Null Handling
    // -------------------------------------------------------

    [Test]
    public void GivenLengthAndNull_WhenCompared_ShouldReturnFalse()
    {
        var length = new Feet(1);

        Assert.That(length.Equals(null), Is.False);
    }

    // -------------------------------------------------------
    // 7. Same Reference (Reflexive)
    // -------------------------------------------------------

    [Test]
    public void GivenSameReference_WhenCompared_ShouldReturnTrue()
    {
        var length = new Feet(1);

        Assert.That(length.Equals(length), Is.True);
    }

    // -------------------------------------------------------
    // 8. Different Object Type Safety
    // -------------------------------------------------------

    [Test]
    public void GivenLengthAndDifferentObjectType_WhenCompared_ShouldReturnFalse()
    {
        var length = new Feet(1);
        object randomObject = new object();

        Assert.That(length.Equals(randomObject), Is.False);
    }

    // -------------------------------------------------------
    // 9. Different Values Same Unit
    // -------------------------------------------------------

    [Test]
    public void GivenDifferentFeetValues_WhenCompared_ShouldReturnFalse()
    {
        Assert.That(new Feet(1).Equals(new Feet(2)), Is.False);
    }

    // -------------------------------------------------------
    // 10. Different Values Cross Unit
    // -------------------------------------------------------

    [Test]
    public void GivenEquivalentLookingButDifferentValues_WhenCompared_ShouldReturnFalse()
    {
        Assert.That(new Inches(11).Equals(new Feet(1)), Is.False);
    }
}