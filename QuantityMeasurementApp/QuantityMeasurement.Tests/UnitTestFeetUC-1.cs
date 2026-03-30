using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Units;
using QuantityMeasurement.ModelLayerLayer.Core;

namespace QuantityMeasurement.Tests;

[TestFixture]
public class FeetTests
{
    [Test]
    public void Given1FeetAnd1Feet_WhenCompared_ShouldReturnTrue()
    {
        // Arrange
        Feet f1 = new Feet(1.0);
        Feet f2 = new Feet(1.0);

        // Act
        bool result = f1.Equals(f2);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Given1FeetAnd2Feet_WhenCompared_ShouldReturnFalse()
    {
        // Arrange
        Feet f1 = new Feet(1.0);
        Feet f2 = new Feet(2.0);
        Assert.That(f1.Equals(f2), Is.False); // Act and Assert combined
    }

    [Test]
    public void Given1FeetAndNull_WhenCompared_ShouldReturnFalse()
    {
        Feet f1 = new Feet(1.0);// Arrange
        Assert.That(f1.Equals(null), Is.False); // Act and Assert combined
    }

    [Test]
    public void GivenSameFeetReference_WhenCompared_ShouldReturnTrue()
    {
        Feet f1 = new Feet(1.0); // Arrange
        // Reflexive property test
        Assert.That(f1.Equals(f1), Is.True); // Act and Assert combined
    }
}