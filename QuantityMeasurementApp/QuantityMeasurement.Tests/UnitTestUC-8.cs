using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Units;
using QuantityMeasurement.ModelLayerLayer.Core;
using System;

namespace QuantityMeasurementApp.Tests
{
    [TestFixture]
    public class LengthUnitRefactoredUC8Tests
    {
        private const double Epsilon = 1e-6;

        // ============================================================
        // 1–4  Standalone Unit Constants
        // ============================================================

        [Test]
        public void testLengthUnitEnum_FeetConstant()
        {
            Assert.That(Unit.Feet.ConversionFactorToBase, Is.EqualTo(1.0));
        }

        [Test]
        public void testLengthUnitEnum_InchesConstant()
        {
            Assert.That(Unit.Inch.ConversionFactorToBase, 
                        Is.EqualTo(1.0 / 12.0).Within(Epsilon));
        }

        [Test]
        public void testLengthUnitEnum_YardsConstant()
        {
            Assert.That(Unit.Yard.ConversionFactorToBase, 
                        Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testLengthUnitEnum_CentimetersConstant()
        {
            Assert.That(Unit.Centimeter.ConversionFactorToBase, 
                        Is.EqualTo(0.0328084).Within(Epsilon));
        }

        // ============================================================
        // 5–8 ConvertToBaseUnit()
        // ============================================================

        [Test]
        public void testConvertToBaseUnit_FeetToFeet()
        {
            Assert.That(Unit.Feet.ConvertToBaseUnit(5.0),
                        Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_InchesToFeet()
        {
            Assert.That(Unit.Inch.ConvertToBaseUnit(12.0),
                        Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_YardsToFeet()
        {
            Assert.That(Unit.Yard.ConvertToBaseUnit(1.0),
                        Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_CentimetersToFeet()
        {
            Assert.That(Unit.Centimeter.ConvertToBaseUnit(30.48),
                        Is.EqualTo(1.0).Within(1e-4));
        }

        // ============================================================
        // 9–12 ConvertFromBaseUnit()
        // ============================================================

        [Test]
        public void testConvertFromBaseUnit_FeetToFeet()
        {
            Assert.That(Unit.Feet.ConvertFromBaseUnit(2.0),
                        Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_FeetToInches()
        {
            Assert.That(Unit.Inch.ConvertFromBaseUnit(1.0),
                        Is.EqualTo(12.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_FeetToYards()
        {
            Assert.That(Unit.Yard.ConvertFromBaseUnit(3.0),
                        Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_FeetToCentimeters()
        {
            Assert.That(Unit.Centimeter.ConvertFromBaseUnit(1.0),
                        Is.EqualTo(30.48).Within(1e-4));
        }

        // ============================================================
        // 13–16 Quantity Delegation Verification
        // ============================================================

        [Test]
        public void testQuantityLengthRefactored_Equality()
        {
            Assert.That(new Feet(1.0)
                .Equals(new Inches(12.0)), Is.True);
        }

        [Test]
        public void testQuantityLengthRefactored_ConvertTo()
        {
            var result = new Feet(1.0).ConvertTo(Unit.Inch);

            Assert.That(result.value, Is.EqualTo(12.0).Within(Epsilon));
        }

        [Test]
        public void testQuantityLengthRefactored_Add()
        {
            var result = new Feet(1.0)
                .Add(new Inches(12.0), Unit.Feet);

            Assert.That(result.value, Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testQuantityLengthRefactored_AddWithTargetUnit()
        {
            var result = new Feet(1.0)
                .Add(new Inches(12.0), Unit.Yard);

            Assert.That(result.value, Is.EqualTo(2.0 / 3.0).Within(Epsilon));
        }

        // ============================================================
        // 17–19 Type Safety & Validation
        // ============================================================

        [Test]
        public void testQuantityLengthRefactored_NullUnit()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var x = new Feet(1.0).ConvertTo(null);
            });
        }

        [Test]
        public void testQuantityLengthRefactored_InvalidValue()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var x = new Feet(double.NaN);
            });
        }

        [Test]
        public void testQuantityLengthRefactored_InvalidInfinity()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var x = new Feet(double.PositiveInfinity);
            });
        }

        // ============================================================
        // 20 Round Trip Accuracy
        // ============================================================

        [Test]
        public void testRoundTripConversion_RefactoredDesign()
        {
            double original = 5.75;

            var first = new Feet(original);
            var converted = first.ConvertTo(Unit.Inch);
            var back = converted.ConvertTo(Unit.Feet);

            Assert.That(back.value, Is.EqualTo(original).Within(Epsilon));
        }

        // ============================================================
        // 21 Architectural Scalability Pattern
        // ============================================================

        [Test]
        public void testArchitecturalScalability_MultipleCategories()
        {
            // Ensures Unit is standalone and not nested
            Assert.That(typeof(Unit).IsNested, Is.False);
        }

        // ============================================================
        // 22 Unit Immutability
        // ============================================================

        [Test]
        public void testUnitImmutability()
        {
            Assert.That(Unit.Feet.Name, Is.EqualTo("Feet"));
        }

        // ============================================================
        // 23 Equality Contract Preservation
        // ============================================================

        [Test]
        public void testEqualityContract_Transitive()
        {
            var a = new Feet(3);
            var b = new Yard(1);
            var c = new Inches(36);

            Assert.That(a.Equals(b), Is.True);
            Assert.That(b.Equals(c), Is.True);
            Assert.That(a.Equals(c), Is.True);
        }

        // ============================================================
        // 24 Backward Compatibility Sanity
        // ============================================================

        [Test]
        public void testBackwardCompatibility_SanityCheck()
        {
            var result = new Feet(1)
                .Add(new Inches(12));

            Assert.That(result.value, Is.EqualTo(2.0).Within(Epsilon));
        }
    }
}