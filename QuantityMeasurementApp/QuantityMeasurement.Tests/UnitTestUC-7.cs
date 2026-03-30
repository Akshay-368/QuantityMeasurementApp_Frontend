using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Units;
using QuantityMeasurement.ModelLayerLayer.Core;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestFixture]
    public class LengthAdditionExplicitTargetUC7Tests
    {
        private const double Epsilon = 1e-6;

        // --------------------------------------------------
        // Explicit Target Unit Tests
        // --------------------------------------------------

        [Test]
        public void TestAddition_ExplicitTargetUnit_Feet()
        {
            var result = new Feet(1.0)
                .Add(new Inches(12.0), Unit.Feet);

            Assert.That(result.value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.unit, Is.EqualTo(Unit.Feet));
        }

        [Test]
        public void TestAddition_ExplicitTargetUnit_Inches()
        {
            var result = new Feet(1.0)
                .Add(new Inches(12.0), Unit.Inch);

            Assert.That(result.value, Is.EqualTo(24.0).Within(Epsilon));
            Assert.That(result.unit, Is.EqualTo(Unit.Inch));
        }

        [Test]
        public void TestAddition_ExplicitTargetUnit_Yards()
        {
            var result = new Feet(1.0)
                .Add(new Inches(12.0), Unit.Yard);

            Assert.That(result.value, Is.EqualTo(2.0 / 3.0).Within(Epsilon));
            Assert.That(result.unit, Is.EqualTo(Unit.Yard));
        }

        [Test]
        public void TestAddition_ExplicitTargetUnit_Centimeters()
        {
            var result = new Inches(1.0)
                .Add(new Inches(1.0), Unit.Centimeter);

            Assert.That(result.value, Is.EqualTo(5.08).Within(1e-4));
            Assert.That(result.unit, Is.EqualTo(Unit.Centimeter));
        }

        // --------------------------------------------------
        // Target Unit Consistency
        // --------------------------------------------------

        [Test]
        public void TestAddition_TargetUnitAlwaysMatchesSpecifiedUnit()
        {
            var result = new Yard(2.0)
                .Add(new Feet(3.0), Unit.Feet);

            Assert.That(result.unit, Is.EqualTo(Unit.Feet));
        }

        // --------------------------------------------------
        // Commutativity
        // --------------------------------------------------

        [Test]
        public void TestAddition_ExplicitTargetUnit_Commutative()
        {
            var result1 = new Feet(1.0)
                .Add(new Inches(12.0), Unit.Yard);

            var result2 = new Inches(12.0)
                .Add(new Feet(1.0), Unit.Yard);

            Assert.That(result1.value, Is.EqualTo(result2.value).Within(Epsilon));
            Assert.That(result1.unit, Is.EqualTo(Unit.Yard));
            Assert.That(result2.unit, Is.EqualTo(Unit.Yard));
        }

        // --------------------------------------------------
        // Mathematical Consistency Across Target Units
        // --------------------------------------------------

        [Test]
        public void TestAddition_MathematicalCorrectnessAcrossUnits()
        {
            var feetResult = new Feet(1.0)
                .Add(new Inches(12.0), Unit.Feet);

            var inchResult = new Feet(1.0)
                .Add(new Inches(12.0), Unit.Inch);

            Assert.That(feetResult.value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(inchResult.value, Is.EqualTo(24.0).Within(Epsilon));
        }

        // --------------------------------------------------
        // Edge Cases
        // --------------------------------------------------

        [Test]
        public void TestAddition_WithZero()
        {
            var result = new Feet(5.0)
                .Add(new Inches(0.0), Unit.Yard);

            Assert.That(result.value, Is.EqualTo(5.0 / 3.0).Within(Epsilon));
        }

        [Test]
        public void TestAddition_NegativeValues()
        {
            var result = new Feet(5.0)
                .Add(new Feet(-2.0), Unit.Inch);

            Assert.That(result.value, Is.EqualTo(36.0).Within(Epsilon));
        }

        [Test]
        public void TestAddition_LargeToSmallScale()
        {
            var result = new Feet(1000.0)
                .Add(new Feet(500.0), Unit.Inch);

            Assert.That(result.value, Is.EqualTo(18000.0).Within(Epsilon));
        }

        [Test]
        public void TestAddition_SmallToLargeScale()
        {
            var result = new Inches(12.0)
                .Add(new Inches(12.0), Unit.Yard);

            Assert.That(result.value, Is.EqualTo(2.0 / 3.0).Within(Epsilon));
        }

        // --------------------------------------------------
        // Null Target Unit Handling
        // --------------------------------------------------

        [Test]
        public void TestAddition_NullTargetUnit_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Feet(1.0)
                    .Add(new Inches(12.0), null);
            });
        }


        // --------------------------------------------------
        // Exhaustive Combination Sanity Test
        // --------------------------------------------------

        [Test]
        public void TestAddition_AllUnitCombinations()
        {
            Length[] values =
            {
                new Feet(1),
                new Inches(12),
                new Yard(1),
                new Meter(1),
                new Centimeter(100)
            };

            Unit[] targets =
            {
                Unit.Feet,
                Unit.Inch,
                Unit.Yard,
                Unit.Meter,
                Unit.Centimeter
            };

            foreach (var first in values)
            {
                foreach (var second in values)
                {
                    foreach (var target in targets)
                    {
                        var result = first.Add(second, target);

                        Assert.That(result.unit, Is.EqualTo(target));
                    }
                }
            }
        }
    }
}