using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Core;
using QuantityMeasurement.ModelLayerLayer.Units;
using System;

namespace QuantityMeasurement.Tests
{
    [TestFixture]
    public class TemperatureMeasurementUC12Tests
    {
        private const double Epsilon = 1e-6;

        // ============================================================
        // 1️⃣ EQUALITY TESTS
        // ============================================================

        [Test]
        public void testTemperatureEquality_CelsiusToCelsius_SameValue()
        {
            Assert.That(new Celsius(0.0)
                .Equals(new Celsius(0.0)), Is.True);
        }

        [Test]
        public void testTemperatureEquality_FahrenheitToFahrenheit_SameValue()
        {
            Assert.That(new Fahrenheit(32.0)
                .Equals(new Fahrenheit(32.0)), Is.True);
        }

        [Test]
        public void testTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
        {
            Assert.That(new Celsius(0.0)
                .Equals(new Fahrenheit(32.0)), Is.True);
        }

        [Test]
        public void testTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit()
        {
            Assert.That(new Celsius(100.0)
                .Equals(new Fahrenheit(212.0)), Is.True);
        }

        [Test]
        public void testTemperatureEquality_CelsiusToFahrenheit_Negative40Equal()
        {
            Assert.That(new Celsius(-40.0)
                .Equals(new Fahrenheit(-40.0)), Is.True);
        }

        [Test]
        public void testTemperatureEquality_SymmetricProperty()
        {
            var a = new Celsius(0.0);
            var b = new Fahrenheit(32.0);

            Assert.That(a.Equals(b), Is.True);
            Assert.That(b.Equals(a), Is.True);
        }

        [Test]
        public void testTemperatureEquality_ReflexiveProperty()
        {
            var temp = new Kelvin(273.15);
            Assert.That(temp.Equals(temp), Is.True);
        }

        [Test]
        public void testTemperatureDifferentValuesInequality()
        {
            Assert.That(new Celsius(50.0)
                .Equals(new Celsius(100.0)), Is.False);
        }

        [Test]
        public void testTemperatureNullOperandValidation_InComparison()
        {
            Assert.That(new Celsius(50.0)
                .Equals(null), Is.False);
        }

        // ============================================================
        // 2️⃣ CONVERSION TESTS
        // ============================================================

        [Test]
        public void testTemperatureConversion_CelsiusToFahrenheit_VariousValues()
        {
            Assert.That(new Celsius(50)
                .ConvertTo(Unit.Fahrenheit).value,
                Is.EqualTo(122).Within(Epsilon));

            Assert.That(new Celsius(-20)
                .ConvertTo(Unit.Fahrenheit).value,
                Is.EqualTo(-4).Within(Epsilon));
        }

        [Test]
        public void testTemperatureConversion_FahrenheitToCelsius_VariousValues()
        {
            Assert.That(new Fahrenheit(122)
                .ConvertTo(Unit.Celsius).value,
                Is.EqualTo(50).Within(Epsilon));

            Assert.That(new Fahrenheit(-4)
                .ConvertTo(Unit.Celsius).value,
                Is.EqualTo(-20).Within(Epsilon));
        }

        [Test]
        public void testTemperatureConversion_KelvinToCelsius()
        {
            Assert.That(new Kelvin(273.15)
                .ConvertTo(Unit.Celsius).value,
                Is.EqualTo(0).Within(Epsilon));
        }

        [Test]
        public void testTemperatureConversion_CelsiusToKelvin()
        {
            Assert.That(new Celsius(0)
                .ConvertTo(Unit.Kelvin).value,
                Is.EqualTo(273.15).Within(Epsilon));
        }

        [Test]
        public void testTemperatureConversion_RoundTrip_PreservesValue()
        {
            var original = new Celsius(37.0);

            var roundTrip = original
                .ConvertTo(Unit.Fahrenheit)
                .ConvertTo(Unit.Celsius);

            Assert.That(roundTrip.value,
                Is.EqualTo(37.0).Within(Epsilon));
        }

        [Test]
        public void testTemperatureConversion_SameUnit()
        {
            var result = new Celsius(25)
                .ConvertTo(Unit.Celsius);

            Assert.That(result.value,
                Is.EqualTo(25).Within(Epsilon));
        }

        [Test]
        public void testTemperatureConversion_ZeroValue()
        {
            Assert.That(new Celsius(0)
                .ConvertTo(Unit.Fahrenheit).value,
                Is.EqualTo(32).Within(Epsilon));
        }

        [Test]
        public void testTemperatureConversion_LargeValues()
        {
            Assert.That(new Celsius(1000)
                .ConvertTo(Unit.Fahrenheit).value,
                Is.EqualTo(1832).Within(Epsilon));
        }

        // ============================================================
        // 3️⃣ ADD / SUB / DIV TESTS
        // ============================================================

        [Test]
        public void testTemperatureAddition_SameUnit()
        {
            var result = new Celsius(10)
                .Add(new Celsius(20));

            Assert.That(result.value,
                Is.EqualTo(30).Within(Epsilon));
        }

        [Test]
        public void testTemperatureAddition_CrossUnit()
        {
            var result = new Celsius(0)
                .Add(new Fahrenheit(32));

            Assert.That(result.value,
                Is.EqualTo(0).Within(Epsilon));
        }

        [Test]
        public void testTemperatureSubtraction()
        {
            var result = new Celsius(100)
                .Subtract(new Celsius(50));

            Assert.That(result.value,
                Is.EqualTo(50).Within(Epsilon));
        }

        [Test]
        public void testTemperatureDivision_ByScalar()
        {
            var result = new Celsius(100)
                .Divide(2);

            Assert.That(result.value,
                Is.EqualTo(50).Within(Epsilon));
        }

        [Test]
        public void testTemperatureDivision_ByTemperature()
        {
            var result = new Celsius(100)
                .Divide(new Celsius(50));

            Assert.That(result,
                Is.EqualTo(2.0).Within(Epsilon));
        }

        // ============================================================
        // 4️⃣ CROSS CATEGORY INCOMPATIBILITY
        // ============================================================

        [Test]
        public void testTemperatureVsLengthIncompatibility()
        {
            Assert.That(new Celsius(100)
                .Equals(new Feet(100)), Is.False);
        }

        [Test]
        public void testTemperatureVsWeightIncompatibility()
        {
            Assert.That(new Celsius(50)
                .Equals(new Kilogram(50)), Is.False);
        }

        [Test]
        public void testTemperatureVsVolumeIncompatibility()
        {
            Assert.That(new Celsius(25)
                .Equals(new Litre(25)), Is.False);
        }

        // ============================================================
        // 5️⃣ PRECISION & EDGE CASES
        // ============================================================

        [Test]
        public void testTemperatureConversionPrecision_Epsilon()
        {
            var result = new Celsius(0.000001)
                .ConvertTo(Unit.Fahrenheit);

            Assert.That(result.value,
                Is.EqualTo(32.0000018).Within(1e-5));
        }

        [Test]
        public void testTemperatureConversionEdgeCase_VerySmallDifference()
        {
            var a = new Celsius(100.000001);
            var b = new Fahrenheit(212.0000018);

            Assert.That(a.Equals(b), Is.True);
        }

        // ============================================================
        // 6️⃣ ARCHITECTURAL BACKWARD COMPATIBILITY
        // ============================================================

        [Test]
        public void testTemperatureBackwardCompatibility_LengthAndWeightStillWork()
        {
            Assert.That(new Feet(1)
                .Equals(new Inches(12)), Is.True);

            Assert.That(new Kilogram(1)
                .Equals(new Gram(1000)), Is.True);
        }

        [Test]
        public void testTemperatureIntegration_BaseTypeCheck()
        {
            Assert.That(typeof(Celsius).BaseType,
                Is.EqualTo(typeof(Temperature)));
        }
    }
}