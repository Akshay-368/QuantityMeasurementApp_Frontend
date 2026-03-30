using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Units;
using QuantityMeasurement.ModelLayerLayer.Core;
using System;

namespace QuantityMeasurement.Tests
{
    [TestFixture]
    public class LengthAdditionUC6Tests
    {
        private const double Epsilon = 1e-6;

        // --------------------------------------------------
        // Same Unit Addition
        // --------------------------------------------------

        [Test]
        public void testAddition_SameUnit_FeetPlusFeet()
        {
            var result = new Feet(1.0).Add(new Feet(2.0));

            Assert.That(result.value, Is.EqualTo(3.0).Within(Epsilon));
            Assert.That(result.unit, Is.EqualTo(Unit.Feet));
        }

        [Test]
        public void testAddition_SameUnit_InchPlusInch()
        {
            var result = new Inches(6.0).Add(new Inches(6.0));

            Assert.That(result.value, Is.EqualTo(12.0).Within(Epsilon));
            Assert.That(result.unit, Is.EqualTo(Unit.Inch));
        }

        // --------------------------------------------------
        // Cross Unit Addition
        // --------------------------------------------------

        [Test]
        public void testAddition_CrossUnit_FeetPlusInches()
        {
            var result = new Feet(1.0).Add(new Inches(12.0));

            Assert.That(result.value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.unit, Is.EqualTo(Unit.Feet));
        }

        [Test]
        public void testAddition_CrossUnit_InchPlusFeet()
        {
            var result = new Inches(12.0).Add(new Feet(1.0));

            Assert.That(result.value, Is.EqualTo(24.0).Within(Epsilon));
            Assert.That(result.unit, Is.EqualTo(Unit.Inch));
        }

        [Test]
        public void testAddition_CrossUnit_YardPlusFeet()
        {
            var result = new Yard(1.0).Add(new Feet(3.0));

            Assert.That(result.value, Is.EqualTo(2.0).Within(Epsilon));
            Assert.That(result.unit, Is.EqualTo(Unit.Yard));
        }

        [Test]
        public void testAddition_CrossUnit_CentimeterPlusInch()
        {
            var result = new Centimeter(2.54).Add(new Inches(1.0));

            Assert.That(result.value, Is.EqualTo(5.08).Within(1e-4));
            Assert.That(result.unit, Is.EqualTo(Unit.Centimeter));
        }

        // --------------------------------------------------
        // Commutativity (Base Value Equality)
        // --------------------------------------------------

        [Test]
        public void testAddition_Commutativity_BaseValue()
        {
            var a = new Feet(1.0);
            var b = new Inches(12.0);

            var result1 = a.Add(b);
            var result2 = b.Add(a);

            Assert.That(result1.ConvertTo(Unit.Feet).value,
                        Is.EqualTo(result2.ConvertTo(Unit.Feet).value).Within(Epsilon));
        }

        // --------------------------------------------------
        // Identity Element
        // --------------------------------------------------

        [Test]
        public void testAddition_WithZero()
        {
            var result = new Feet(5.0).Add(new Inches(0.0));

            Assert.That(result.value, Is.EqualTo(5.0).Within(Epsilon));
            Assert.That(result.unit, Is.EqualTo(Unit.Feet));
        }

        // --------------------------------------------------
        // Negative Values
        // --------------------------------------------------

        [Test]
        public void testAddition_NegativeValues()
        {
            var result = new Feet(5.0).Add(new Feet(-2.0));

            Assert.That(result.value, Is.EqualTo(3.0).Within(Epsilon));
        }

        // --------------------------------------------------
        // Null Handling
        // --------------------------------------------------

        [Test]
        public void testAddition_NullSecondOperand()
        {
            var first = new Feet(1.0);

            Assert.Throws<ArgumentNullException>(() =>
            {
                first.Add(null!);
            });
        }

        // --------------------------------------------------
        // Large Values
        // --------------------------------------------------

        [Test]
        public void testAddition_LargeValues()
        {
            var result = new Feet(1e6).Add(new Feet(1e6));

            Assert.That(result.value, Is.EqualTo(2e6).Within(Epsilon));
        }

        // --------------------------------------------------
        // Small Values
        // --------------------------------------------------

        [Test]
        public void testAddition_SmallValues()
        {
            var result = new Feet(0.001).Add(new Feet(0.002));

            Assert.That(result.value, Is.EqualTo(0.003).Within(Epsilon));
        }

        // --------------------------------------------------
        // Precision Round Trip
        // --------------------------------------------------

        [Test]
        public void testAddition_RoundTripPrecision()
        {
            var a = new Feet(3.75);
            var b = new Inches(9);

            var result = a.Add(b);

            double baseSum =
                a.ConvertTo(Unit.Feet).value +
                b.ConvertTo(Unit.Feet).value;

            Assert.That(result.value, Is.EqualTo(baseSum).Within(Epsilon));
        }

        // --------------------------------------------------
        // Result Unit Consistency
        // --------------------------------------------------

        [Test]
        public void testAddition_ResultUnitMatchesFirstOperand()
        {
            var first = new Meter(1.0);
            var second = new Feet(3.28084);

            var result = first.Add(second);

            Assert.That(result.unit, Is.EqualTo(Unit.Meter));
        }

        // --------------------------------------------------
        // Immutability Check
        // --------------------------------------------------

        [Test]
        public void testAddition_OriginalObjectsRemainUnchanged()
        {
            var a = new Feet(2.0);
            var b = new Feet(3.0);

            var result = a.Add(b);

            Assert.That(a.value, Is.EqualTo(2.0));
            Assert.That(b.value, Is.EqualTo(3.0));
            Assert.That(result.value, Is.EqualTo(5.0));
        }
    }
}