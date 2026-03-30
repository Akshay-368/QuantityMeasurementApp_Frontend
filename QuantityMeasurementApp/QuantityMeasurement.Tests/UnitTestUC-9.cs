using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Units;
using QuantityMeasurement.ModelLayerLayer.Core;
using System;
using System.Collections.Generic;

namespace QuantityMeasurement.Tests
{
    [TestFixture]
    public class WeightMeasurementUC9Tests
    {
        private const double Epsilon = 1e-6;

        // --------------------------------------------------
        // 1️ SAME UNIT EQUALITY
        // --------------------------------------------------

        [Test]
        public void testEquality_KilogramToKilogram_SameValue()
        {
            Assert.That(new Kilogram(1.0).Equals(new Kilogram(1.0)), Is.True);
        }

        [Test]
        public void testEquality_KilogramToKilogram_DifferentValue()
        {
            Assert.That(new Kilogram(1.0).Equals(new Kilogram(2.0)), Is.False);
        }

        [Test]
        public void testEquality_GramToGram_SameValue()
        {
            Assert.That(new Gram(500).Equals(new Gram(500)), Is.True);
        }

        [Test]
        public void testEquality_PoundToPound_SameValue()
        {
            Assert.That(new Pound(2).Equals(new Pound(2)), Is.True);
        }

        // --------------------------------------------------
        // 2️ CROSS UNIT EQUALITY
        // --------------------------------------------------

        [Test]
        public void testEquality_KilogramToGram_EquivalentValue()
        {
            Assert.That(new Kilogram(1.0).Equals(new Gram(1000.0)), Is.True);
        }

        [Test]
        public void testEquality_GramToKilogram_EquivalentValue()
        {
            Assert.That(new Gram(1000.0).Equals(new Kilogram(1.0)), Is.True);
        }

        [Test]
        public void testEquality_KilogramToPound_EquivalentValue()
        {
            Assert.That(new Kilogram(1.0)
                .Equals(new Pound(2.20462)), Is.True);
        }

        [Test]
        public void testEquality_GramToPound_EquivalentValue()
        {
            Assert.That(new Gram(453.592)
                .Equals(new Pound(1.0)), Is.True);
        }

        // --------------------------------------------------
        // 3️ CATEGORY SAFETY
        // --------------------------------------------------

        [Test]
        public void testEquality_WeightVsLength_Incompatible()
        {
            Assert.That(new Kilogram(1.0)
                .Equals(new Feet(1.0)), Is.False);
        }

        [Test]
        public void testEquality_NullComparison()
        {
            Assert.That(new Kilogram(1.0)
                .Equals(null), Is.False);
        }

        [Test]
        public void testEquality_SameReference()
        {
            var kg = new Kilogram(1.0);
            Assert.That(kg.Equals(kg), Is.True);
        }

        // --------------------------------------------------
        // 4️ ZERO / NEGATIVE / LARGE VALUES
        // --------------------------------------------------

        [Test]
        public void testEquality_ZeroValue()
        {
            Assert.That(new Kilogram(0)
                .Equals(new Gram(0)), Is.True);
        }

        [Test]
        public void testEquality_NegativeWeight()
        {
            Assert.That(new Kilogram(-1)
                .Equals(new Gram(-1000)), Is.True);
        }

        [Test]
        public void testEquality_LargeWeightValue()
        {
            Assert.That(new Gram(1_000_000)
                .Equals(new Kilogram(1000)), Is.True);
        }

        // --------------------------------------------------
        // 5️ CONVERSION TESTS
        // --------------------------------------------------

        [Test]
        public void testConversion_KilogramToGram()
        {
            var result = new Kilogram(1.0)
                .ConvertTo(Unit.Gram);

            Assert.That(result.value, Is.EqualTo(1000).Within(Epsilon));
        }

        [Test]
        public void testConversion_GramToKilogram()
        {
            var result = new Gram(1000)
                .ConvertTo(Unit.Kilogram);

            Assert.That(result.value, Is.EqualTo(1).Within(Epsilon));
        }

        [Test]
        public void testConversion_KilogramToPound()
        {
            var result = new Kilogram(1)
                .ConvertTo(Unit.Pound);

            Assert.That(result.value, Is.EqualTo(2.20462).Within(1e-5));
        }

        [Test]
        public void testConversion_PoundToKilogram()
        {
            var result = new Pound(2.20462)
                .ConvertTo(Unit.Kilogram);

            Assert.That(result.value, Is.EqualTo(1).Within(1e-5));
        }

        [Test]
        public void testConversion_RoundTrip()
        {
            var original = new Kilogram(1.5);

            var roundTrip = original
                .ConvertTo(Unit.Gram)
                .ConvertTo(Unit.Kilogram);

            Assert.That(roundTrip.value,
                Is.EqualTo(1.5).Within(Epsilon));
        }

        // --------------------------------------------------
        // 6️ ADDITION (IMPLICIT TARGET)
        // --------------------------------------------------

        [Test]
        public void testAddition_SameUnit_KilogramPlusKilogram()
        {
            var result = new Kilogram(1)
                .Add(new Kilogram(2));

            Assert.That(result.value, Is.EqualTo(3).Within(Epsilon));
        }

        [Test]
        public void testAddition_CrossUnit_KilogramPlusGram()
        {
            var result = new Kilogram(1)
                .Add(new Gram(1000));

            Assert.That(result.value, Is.EqualTo(2).Within(Epsilon));
        }

        [Test]
        public void testAddition_CrossUnit_PoundPlusKilogram()
        {
            var result = new Pound(2.20462)
                .Add(new Kilogram(1));

            Assert.That(result.value,
                Is.EqualTo(4.40924).Within(1e-4));
        }

        // --------------------------------------------------
        // 7️ ADDITION (EXPLICIT TARGET)
        // --------------------------------------------------

        [Test]
        public void testAddition_ExplicitTargetUnit_Gram()
        {
            var result = new Kilogram(1)
                .Add(new Gram(1000), Unit.Gram);

            Assert.That(result.value, Is.EqualTo(2000).Within(Epsilon));
        }

        [Test]
        public void testAddition_Commutativity()
        {
            var result1 = new Kilogram(1)
                .Add(new Gram(1000), Unit.Kilogram);

            var result2 = new Gram(1000)
                .Add(new Kilogram(1), Unit.Kilogram);

            Assert.That(result1.value,
                Is.EqualTo(result2.value).Within(Epsilon));
        }

        // --------------------------------------------------
        // 8️ EDGE CASES
        // --------------------------------------------------

        [Test]
        public void testAddition_WithZero()
        {
            var result = new Kilogram(5)
                .Add(new Gram(0));

            Assert.That(result.value, Is.EqualTo(5).Within(Epsilon));
        }

        [Test]
        public void testAddition_NegativeValues()
        {
            var result = new Kilogram(5)
                .Add(new Gram(-2000));

            Assert.That(result.value, Is.EqualTo(3).Within(Epsilon));
        }

        [Test]
        public void testAddition_LargeValues()
        {
            var result = new Kilogram(1e6)
                .Add(new Kilogram(1e6));

            Assert.That(result.value, Is.EqualTo(2e6).Within(Epsilon));
        }
    }
}
