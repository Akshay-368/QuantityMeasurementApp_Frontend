using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Units;
using QuantityMeasurement.ModelLayerLayer.Core;
using System;

namespace QuantityMeasurementApp.Tests
{
    public class LengthConversionTests
    {
        private const double Epsilon = 1e-6;

        // --------------------------------------------------
        // Basic Unit Conversion
        // --------------------------------------------------

        [Test]
        public void TestConversion_FeetToInches()
        {
            var feet = new Feet(1.0);
            var result = feet.ConvertTo(Unit.Inch);

            Assert.That(result.value, Is.EqualTo(12.0).Within(Epsilon));
        }

        [Test]
        public void TestConversion_InchesToFeet()
        {
            var inches = new Inches(24.0);
            var result = inches.ConvertTo(Unit.Feet);

            Assert.That(result.value, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void TestConversion_YardsToFeet()
        {
            var yards = new Yard(1.0);
            var result = yards.ConvertTo(Unit.Feet);

            Assert.That(result.value, Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void TestConversion_FeetToYards()
        {
            var feet = new Feet(6.0);
            var result = feet.ConvertTo(Unit.Yard);

            Assert.That(result.value, Is.EqualTo(2.0).Within(Epsilon));
        }

        // --------------------------------------------------
        // Cross Unit Conversion
        // --------------------------------------------------

        [Test]
        public void TestConversion_YardsToInches()
        {
            var yards = new Yard(1.0);
            var result = yards.ConvertTo(Unit.Inch);

            Assert.That(result.value, Is.EqualTo(36.0).Within(Epsilon));
        }

        [Test]
        public void TestConversion_InchesToYards()
        {
            var inches = new Inches(72.0);
            var result = inches.ConvertTo(Unit.Yard);

            Assert.That(result.value, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void TestConversion_CentimetersToInches()
        {
            var cm = new Centimeter(2.54);
            var result = cm.ConvertTo(Unit.Inch);

            Assert.That(result.value, Is.EqualTo(1.0).Within(1e-4));
        }

        // --------------------------------------------------
        // Round Trip
        // --------------------------------------------------

        [Test]
        public void TestConversion_RoundTrip_PreservesValue()
        {
            double original = 5.5;

            var feet = new Feet(original);
            var inches = feet.ConvertTo(Unit.Inch);
            var backToFeet = inches.ConvertTo(Unit.Feet);

            Assert.That(backToFeet.value, Is.EqualTo(original).Within(Epsilon));
        }

        // --------------------------------------------------
        // Zero and Negative
        // --------------------------------------------------

        [Test]
        public void TestConversion_ZeroValue()
        {
            var feet = new Feet(0.0);
            var result = feet.ConvertTo(Unit.Inch);

            Assert.That(result.value, Is.EqualTo(0.0).Within(Epsilon));
        }

        [Test]
        public void TestConversion_NegativeValue()
        {
            var feet = new Feet(-1.0);
            var result = feet.ConvertTo(Unit.Inch);

            Assert.That(result.value, Is.EqualTo(-12.0).Within(Epsilon));
        }

        // --------------------------------------------------
        // Large and Small Values
        // --------------------------------------------------

        [Test]
        public void TestConversion_LargeValue()
        {
            var feet = new Feet(1_000_000);
            var result = feet.ConvertTo(Unit.Inch);

            Assert.That(result.value, Is.EqualTo(12_000_000).Within(Epsilon));
        }

        [Test]
        public void TestConversion_SmallValue()
        {
            var feet = new Feet(0.000001);
            var result = feet.ConvertTo(Unit.Inch);

            Assert.That(result.value, Is.EqualTo(0.000012).Within(1e-10));
        }

        // --------------------------------------------------
        // Same Unit Conversion
        // --------------------------------------------------

        [Test]
        public void TestConversion_SameUnit_ReturnsSameValue()
        {
            var feet = new Feet(5.0);
            var result = feet.ConvertTo(Unit.Feet);

            Assert.That(result.value, Is.EqualTo(5.0).Within(Epsilon));
        }

        // --------------------------------------------------
        // Invalid Unit Handling
        // --------------------------------------------------

        [Test]
        public void TestConversion_InvalidUnit_Throws()
        {
            var feet = new Feet(1.0);

            Assert.Throws<ArgumentNullException>(() => feet.ConvertTo(null));
        }

        // --------------------------------------------------
        // NaN / Infinity Validation
        // --------------------------------------------------

        [Test]
        public void TestConversion_NaN_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var feet = new Feet(double.NaN);
            });
        }

        [Test]
        public void TestConversion_Infinity_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var feet = new Feet(double.PositiveInfinity);
            });
        }

        // --------------------------------------------------
        // Multi-Step Round Trip Accuracy
        // --------------------------------------------------

        [Test]
        public void TestConversion_MultipleSequentialConversions()
        {
            double original = 7.25;

            var feet = new Feet(original);
            var inches = feet.ConvertTo(Unit.Inch);
            var cm = inches.ConvertTo(Unit.Centimeter);
            var backToFeet = cm.ConvertTo(Unit.Feet);

            Assert.That(backToFeet.value, Is.EqualTo(original).Within(1e-4));
        }

        // --------------------------------------------------
        // Mathematical Consistency Check
        // --------------------------------------------------

        [Test]
        public void TestConversion_MathFormulaConsistency()
        {
            double value = 10;

            var feet = new Feet(value);
            var meters = feet.ConvertTo(Unit.Meter);

            double expected = value * 
                (Unit.Feet.ConversionFactorToBase / Unit.Meter.ConversionFactorToBase);

            Assert.That(meters.value, Is.EqualTo(expected).Within(Epsilon));
        }
    }
}