using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Units;
using QuantityMeasurement.ModelLayerLayer.Core;
using System;

namespace QuantityMeasurement.Tests
{
    [TestFixture]
    public class QuantityMeasurementUC12Tests
    {
        private const double Epsilon = 1e-6;

        // ============================================================
        // 🔹 SUBTRACTION TESTS
        // ============================================================

        [Test]
        public void testSubtraction_SameUnit_FeetMinusFeet()
        {
            var result = new Feet(10.0)
                .Subtract(new Feet(5.0));

            Assert.That(result.value, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_SameUnit_LitreMinusLitre()
        {
            var result = new Litre(10.0)
                .Subtract(new Litre(3.0));

            Assert.That(result.value, Is.EqualTo(7.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_CrossUnit_FeetMinusInches()
        {
            var result = new Feet(10.0)
                .Subtract(new Inches(6.0));

            Assert.That(result.value, Is.EqualTo(9.5).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_CrossUnit_InchesMinusFeet()
        {
            var result = new Inches(120.0)
                .Subtract(new Feet(5.0));

            Assert.That(result.value, Is.EqualTo(60.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_ExplicitTargetUnit_Feet()
        {
            var result = new Feet(10.0)
                .Subtract(new Inches(6.0), Unit.Feet);

            Assert.That(result.value, Is.EqualTo(9.5).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_ExplicitTargetUnit_Inches()
        {
            var result = new Feet(10.0)
                .Subtract(new Inches(6.0), Unit.Inch);

            Assert.That(result.value, Is.EqualTo(114.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_ExplicitTargetUnit_Millilitre()
        {
            var result = new Litre(5.0)
                .Subtract(new Litre(2.0), Unit.Millilitre);

            Assert.That(result.value, Is.EqualTo(3000.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_ResultingInNegative()
        {
            var result = new Feet(5.0)
                .Subtract(new Feet(10.0));

            Assert.That(result.value, Is.EqualTo(-5.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_ResultingInZero()
        {
            var result = new Feet(10.0)
                .Subtract(new Inches(120.0));

            Assert.That(result.value, Is.EqualTo(0.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_WithZeroOperand()
        {
            var result = new Feet(5.0)
                .Subtract(new Inches(0.0));

            Assert.That(result.value, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_WithNegativeValues()
        {
            var result = new Feet(5.0)
                .Subtract(new Feet(-2.0));

            Assert.That(result.value, Is.EqualTo(7.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_NonCommutative()
        {
            var a = new Feet(10.0);
            var b = new Feet(5.0);

            Assert.That(a.Subtract(b).value, Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(b.Subtract(a).value, Is.EqualTo(-5.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_WithLargeValues()
        {
            var result = new Kilogram(1e6)
                .Subtract(new Kilogram(5e5));

            Assert.That(result.value, Is.EqualTo(5e5).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_WithSmallValues()
        {
            var result = new Feet(0.001)
                .Subtract(new Feet(0.0005));

            Assert.That(result.value, Is.EqualTo(0.0005).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_NullOperand()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Feet(10.0).Subtract(null!));
        }

        [Test]
        public void testSubtraction_NullTargetUnit()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Feet(10.0).Subtract(new Feet(5.0), null!));
        }

        [Test]
        public void testSubtraction_CrossCategory()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new Feet(10.0).Subtract(new Kilogram(5.0)));
        }

        [Test]
        public void testSubtraction_ChainedOperations()
        {
            var result = new Feet(10.0)
                .Subtract(new Feet(2.0))
                .Subtract(new Feet(1.0));

            Assert.That(result.value, Is.EqualTo(7.0).Within(Epsilon));
        }

        [Test]
        public void testSubtraction_Immutability()
        {
            var original = new Feet(10.0);
            var result = original.Subtract(new Feet(5.0));

            Assert.That(original.value, Is.EqualTo(10.0));
            Assert.That(result.value, Is.EqualTo(5.0));
        }

        // ============================================================
        // 🔹 DIVISION TESTS
        // ============================================================

        [Test]
        public void testDivision_SameUnit_FeetDividedByFeet()
        {
            var result = new Feet(10.0)
                .Divide(new Feet(2.0));

            Assert.That(result, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_CrossUnit_FeetDividedByInches()
        {
            var result = new Inches(24.0)
                .Divide(new Feet(2.0));

            Assert.That(result, Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_CrossUnit_KilogramDividedByGram()
        {
            var result = new Kilogram(2.0)
                .Divide(new Gram(2000.0));

            Assert.That(result, Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_RatioGreaterThanOne()
        {
            var result = new Feet(10.0)
                .Divide(new Feet(2.0));

            Assert.That(result, Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_RatioLessThanOne()
        {
            var result = new Feet(5.0)
                .Divide(new Feet(10.0));

            Assert.That(result, Is.EqualTo(0.5).Within(Epsilon));
        }

        [Test]
        public void testDivision_RatioEqualToOne()
        {
            var result = new Feet(10.0)
                .Divide(new Feet(10.0));

            Assert.That(result, Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_NonCommutative()
        {
            var a = new Feet(10.0);
            var b = new Feet(5.0);

            Assert.That(a.Divide(b), Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(b.Divide(a), Is.EqualTo(0.5).Within(Epsilon));
        }

        [Test]
        public void testDivision_ByZero()
        {
            Assert.Throws<DivideByZeroException>(() =>
                new Feet(10.0).Divide(new Feet(0.0)));
        }

        [Test]
        public void testDivision_NullOperand()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Feet(10.0).Divide(null!));
        }

        [Test]
        public void testDivision_CrossCategory()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new Feet(10.0).Divide(new Kilogram(5.0)));
        }

        [Test]
        public void testDivision_Immutability()
        {
            var original = new Feet(10.0);
            var result = original.Divide(new Feet(2.0));

            Assert.That(original.value, Is.EqualTo(10.0));
            Assert.That(result, Is.EqualTo(5.0).Within(Epsilon));
        }

        // ============================================================
        // 🔹 INTEGRATION + MATHEMATICAL PROPERTIES
        // ============================================================

        [Test]
        public void testSubtractionAddition_Inverse()
        {
            var a = new Feet(10.0);
            var b = new Feet(3.0);

            var result = a.Add(b)
                          .Subtract(b);

            Assert.That(result.value,
                Is.EqualTo(a.value).Within(Epsilon));
        }

        [Test]
        public void testSubtractionAndDivision_Integration()
        {
            var result = new Feet(10.0)
                .Subtract(new Feet(2.0))
                .Divide(new Feet(2.0));

            Assert.That(result, Is.EqualTo(4.0).Within(Epsilon));
        }

        [Test]
        public void testDivision_PrecisionHandling()
        {
            var result = new Kilogram(1.0)
                .Divide(new Kilogram(1e6));

            Assert.That(result,
                Is.EqualTo(1e-6).Within(Epsilon));
        }
    }
}