using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Units;
using QuantityMeasurement.ModelLayerLayer.Core;
using System;

namespace QuantityMeasurement.Tests
{
    [TestFixture]
    public class VolumeMeasurementUC10Tests
    {
        private const double Epsilon = 1e-6;

        // ============================================================
        // 1️⃣ EQUALITY TESTS
        // ============================================================

        [Test]
        public void testEquality_LitreToLitre_SameValue()
        {
            Assert.That(new Litre(1.0).Equals(new Litre(1.0)), Is.True);
        }

        [Test]
        public void testEquality_LitreToMillilitre_SameValue()
        {
            Assert.That(new Litre(1.0).Equals(new Millilitre(1000.0)), Is.True);
        }

        [Test]
        public void testEquality_LitreToLitre_DifferentValue()
        {
            Assert.That(new Litre(1.0).Equals(new Litre(2.0)), Is.False);
        }

        [Test]
        public void testEquality_LitreToMillilitre_EquivalentValue()
        {
            Assert.That(new Litre(1.0).Equals(new Millilitre(1000.0)), Is.True);
        }

        [Test]
        public void testEquality_MillilitreToLitre_EquivalentValue()
        {
            Assert.That(new Millilitre(1000.0).Equals(new Litre(1.0)), Is.True);
        }

        [Test]
        public void testEquality_LitreToGallon_EquivalentValue()
        {
            Assert.That(new Litre(1.0)
                .Equals(new Gallon(0.264172)), Is.True);
        }

        [Test]
        public void testEquality_GallonToLitre_EquivalentValue()
        {
            Assert.That(new Gallon(1.0)
                .Equals(new Litre(3.78541)), Is.True);
        }

        [Test]
        public void testEquality_VolumeVsLength_Incompatible()
        {
            Assert.That(new Litre(1.0)
                .Equals(new Feet(1.0)), Is.False);
        }

        [Test]
        public void testEquality_VolumeVsWeight_Incompatible()
        {
            Assert.That(new Litre(1.0)
                .Equals(new Kilogram(1.0)), Is.False);
        }

        [Test]
        public void testEquality_NullComparison()
        {
            Assert.That(new Litre(1.0)
                .Equals(null), Is.False);
        }

        [Test]
        public void testEquality_SameReference()
        {
            var litre = new Litre(1.0);
            Assert.That(litre.Equals(litre), Is.True);
        }

        [Test]
        public void testEquality_TransitiveProperty()
        {
            var a = new Litre(1.0);
            var b = new Millilitre(1000.0);
            var c = new Litre(1.0);

            Assert.That(a.Equals(b), Is.True);
            Assert.That(b.Equals(c), Is.True);
            Assert.That(a.Equals(c), Is.True);
        }

        [Test]
        public void testEquality_ZeroValue()
        {
            Assert.That(new Litre(0.0)
                .Equals(new Millilitre(0.0)), Is.True);
        }

        [Test]
        public void testEquality_NegativeVolume()
        {
            Assert.That(new Litre(-1.0)
                .Equals(new Millilitre(-1000.0)), Is.True);
        }

        [Test]
        public void testEquality_LargeVolumeValue()
        {
            Assert.That(new Millilitre(1_000_000.0)
                .Equals(new Litre(1000.0)), Is.True);
        }

        [Test]
        public void testEquality_SmallVolumeValue()
        {
            Assert.That(new Litre(0.001)
                .Equals(new Millilitre(1.0)), Is.True);
        }

        // ============================================================
        // 2️⃣ CONVERSION TESTS
        // ============================================================

        [Test]
        public void testConversion_LitreToMillilitre()
        {
            var result = new Litre(1.0)
                .ConvertTo(Unit.Millilitre);

            Assert.That(result.value, Is.EqualTo(1000).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_LitreToLitre()
        {
            Assert.That(
                Unit.Litre.ConvertToBaseUnit(5.0),
                Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_MillilitreToLitre()
        {
            Assert.That(
                Unit.Millilitre.ConvertToBaseUnit(1000.0),
                Is.EqualTo(1.0).Within(Epsilon));
        }

        [Test]
        public void testConvertToBaseUnit_GallonToLitre()
        {
            Assert.That(
                Unit.Gallon.ConvertToBaseUnit(1.0),
                Is.EqualTo(3.78541).Within(Epsilon));
        }

        [Test]
        public void testConversion_MillilitreToLitre()
        {
            var result = new Millilitre(1000.0)
                .ConvertTo(Unit.Litre);

            Assert.That(result.value, Is.EqualTo(1).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_LitreToLitre()
        {
            Assert.That(
                Unit.Litre.ConvertFromBaseUnit(2.0),
                Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_LitreToMillilitre()
        {
            Assert.That(
                Unit.Millilitre.ConvertFromBaseUnit(1.0),
                Is.EqualTo(1000.0).Within(Epsilon));
        }

        [Test]
        public void testConvertFromBaseUnit_LitreToGallon()
        {
            Assert.That(
                Unit.Gallon.ConvertFromBaseUnit(3.78541),
                Is.EqualTo(1.0).Within(1e-5));
        }

        [Test]
        public void testConversion_GallonToLitre()
        {
            var result = new Gallon(1.0)
                .ConvertTo(Unit.Litre);

            Assert.That(result.value,
                Is.EqualTo(3.78541).Within(Epsilon));
        }

        [Test]
        public void testConversion_LitreToGallon()
        {
            var result = new Litre(3.78541)
                .ConvertTo(Unit.Gallon);

            Assert.That(result.value,
                Is.EqualTo(1.0).Within(1e-5));
        }

        [Test]
        public void testConversion_MillilitreToGallon()
        {
            var result = new Millilitre(1000.0)
                .ConvertTo(Unit.Gallon);

            Assert.That(result.value,
                Is.EqualTo(0.264172).Within(1e-5));
        }

        [Test]
        public void testConversion_SameUnit()
        {
            var result = new Litre(5.0)
                .ConvertTo(Unit.Litre);

            Assert.That(result.value,
                Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_ZeroValue()
        {
            var result = new Litre(0.0)
                .ConvertTo(Unit.Millilitre);

            Assert.That(result.value,
                Is.EqualTo(0.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_NegativeValue()
        {
            var result = new Litre(-1.0)
                .ConvertTo(Unit.Millilitre);

            Assert.That(result.value,
                Is.EqualTo(-1000.0).Within(Epsilon));
        }

        [Test]
        public void testConversion_RoundTrip()
        {
            var original = new Litre(1.5);

            var roundTrip = original
                .ConvertTo(Unit.Millilitre)
                .ConvertTo(Unit.Litre);

            Assert.That(roundTrip.value,
                Is.EqualTo(1.5).Within(Epsilon));
        }

        // ============================================================
        // 3️⃣ ADDITION TESTS
        // ============================================================

        [Test]
        public void testAddition_SameUnit_LitrePlusLitre()
        {
            var result = new Litre(1.0)
                .Add(new Litre(2.0));

            Assert.That(result.value,
                Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Litre()
        {
            var result = new Litre(1.0)
                .Add(new Millilitre(1000.0), Unit.Litre);

            Assert.That(result.value,
                Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Millilitre()
        {
            var result = new Litre(1.0)
                .Add(new Millilitre(1000.0), Unit.Millilitre);

            Assert.That(result.value,
                Is.EqualTo(2000.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_SameUnit_MillilitrePlusMillilitre()
        {
            var result = new Millilitre(500.0)
                .Add(new Millilitre(500.0));

            Assert.That(result.value,
                Is.EqualTo(1000.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_CrossUnit_LitrePlusMillilitre()
        {
            var result = new Litre(1.0)
                .Add(new Millilitre(1000.0));

            Assert.That(result.value,
                Is.EqualTo(2.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_CrossUnit_MillilitrePlusLitre()
        {
            var result = new Millilitre(1000.0)
                .Add(new Litre(1.0));

            Assert.That(result.value,
                Is.EqualTo(2000.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_SmallValues()
        {
            var result = new Litre(0.001)
                .Add(new Litre(0.002));

            Assert.That(result.value,
                Is.EqualTo(0.003).Within(Epsilon));
        }

        [Test]
        public void testAddition_CrossUnit_GallonPlusLitre()
        {
            var result = new Gallon(1.0)
                .Add(new Litre(3.78541));

            Assert.That(result.value,
                Is.EqualTo(2.0).Within(1e-5));
        }

        [Test]
        public void testAddition_ExplicitTargetUnit_Gallon()
        {
            var result = new Litre(3.78541)
                .Add(new Litre(3.78541), Unit.Gallon);

            Assert.That(result.value,
                Is.EqualTo(2.0).Within(1e-5));
        }

        [Test]
        public void testAddition_Commutativity()
        {
            var r1 = new Litre(1.0)
                .Add(new Millilitre(1000.0), Unit.Litre);

            var r2 = new Millilitre(1000.0)
                .Add(new Litre(1.0), Unit.Litre);

            Assert.That(r1.value,
                Is.EqualTo(r2.value).Within(Epsilon));
        }

        [Test]
        public void testAddition_WithZero()
        {
            var result = new Litre(5.0)
                .Add(new Millilitre(0.0));

            Assert.That(result.value,
                Is.EqualTo(5.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_NegativeValues()
        {
            var result = new Litre(5.0)
                .Add(new Millilitre(-2000.0));

            Assert.That(result.value,
                Is.EqualTo(3.0).Within(Epsilon));
        }

        [Test]
        public void testAddition_LargeValues()
        {
            var result = new Litre(1e6)
                .Add(new Litre(1e6));

            Assert.That(result.value,
                Is.EqualTo(2e6).Within(Epsilon));
        }

        // ============================================================
        // 4️⃣ UNIT CONSTANT TESTS
        // ============================================================

        [Test]
        public void testVolumeUnitEnum_LitreConstant()
        {
            Assert.That(Unit.Litre.ConversionFactorToBase,
                Is.EqualTo(1.0));
        }

        [Test]
        public void testVolumeUnitEnum_MillilitreConstant()
        {
            Assert.That(Unit.Millilitre.ConversionFactorToBase,
                Is.EqualTo(0.001).Within(Epsilon));
        }

        [Test]
        public void testVolumeUnitEnum_GallonConstant()
        {
            Assert.That(Unit.Gallon.ConversionFactorToBase,
                Is.EqualTo(3.78541).Within(Epsilon));
        }

        // ============================================================
        // 5️⃣ ARCHITECTURAL INTEGRATION TESTS
        // ============================================================

        [Test]
        public void testScalability_VolumeIntegration()
        {
            Assert.That(typeof(Litre).BaseType,
                Is.EqualTo(typeof(Volume)));
        }

        [Test]
        public void testScalability_VolumeIntegration2()
        {
            Assert.That(typeof(Gallon).BaseType, Is.EqualTo(typeof(Volume)));
        }

        [Test]
        public void testGenericQuantity_VolumeOperations_ShouldWork()
        {
            Assert.That(new Litre(1).Equals(new Litre(1)), Is.True);
        }



        [Test]
        public void testBackwardCompatibility_LengthAndWeightStillWork()
        {
            Assert.That(new Feet(1)
                .Equals(new Inches(12)), Is.True);

            Assert.That(new Kilogram(1)
                .Equals(new Gram(1000)), Is.True);
        }
    }
}