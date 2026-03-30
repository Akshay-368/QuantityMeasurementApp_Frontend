using NUnit.Framework;
using QuantityMeasurement.ModelLayerLayer.Units;
using QuantityMeasurement.ModelLayerLayer.Core;

namespace QuantityMeasurement.Tests
{
    [TestFixture]
    public class QuantityLengthUC4Tests
    {
        // -----------------------------------------------------------
        // Yard to Yard Equality
        // -----------------------------------------------------------

        [Test]
        public void testEquality_YardToYard_SameValue()
        {
            var q1 = new Yard(1.0);
            var q2 = new Yard(1.0);

            Assert.IsTrue(q1.Equals(q2));
        }

        [Test]
        public void testEquality_YardToYard_DifferentValue()
        {
            var q1 = new Yard(1.0);
            var q2 = new Yard(2.0);

            Assert.IsFalse(q1.Equals(q2));
        }

        // -----------------------------------------------------------
        // Cross Unit Yard Conversions
        // -----------------------------------------------------------

        [Test]
        public void testEquality_YardToFeet_EquivalentValue()
        {
            var yard = new Yard(1.0);
            var feet = new Feet(3.0);

            Assert.IsTrue(yard.Equals(feet));
        }

        [Test]
        public void testEquality_FeetToYard_EquivalentValue()
        {
            var feet = new Feet(3.0);
            var yard = new Yard(1.0);

            Assert.IsTrue(feet.Equals(yard));
        }

        [Test]
        public void testEquality_YardToInches_EquivalentValue()
        {
            var yard = new Yard(1.0);
            var inches = new Inches(36.0);

            Assert.IsTrue(yard.Equals(inches));
        }

        [Test]
        public void testEquality_InchesToYard_EquivalentValue()
        {
            var inches = new Inches(36.0);
            var yard = new Yard(1.0);

            Assert.IsTrue(inches.Equals(yard));
        }

        [Test]
        public void testEquality_YardToFeet_NonEquivalentValue()
        {
            var yard = new Yard(1.0);
            var feet = new Feet(2.0);

            Assert.IsFalse(yard.Equals(feet));
        }

        // -----------------------------------------------------------
        // Centimeter Equality
        // -----------------------------------------------------------

        [Test]
        public void testEquality_CentimetersToInches_EquivalentValue()
        {
            var cm = new Centimeter(1.0);
            var inches = new Inches(0.393701);

            Assert.IsTrue(cm.Equals(inches));
        }

        [Test]
        public void testEquality_CentimetersToFeet_NonEquivalentValue()
        {
            var cm = new Centimeter(1.0);
            var feet = new Feet(1.0);

            Assert.IsFalse(cm.Equals(feet));
        }

        // -----------------------------------------------------------
        // Multi Unit Comparisons
        // -----------------------------------------------------------

        [Test]
        public void testEquality_MultiUnit_TransitiveProperty()
        {
            var yard = new Yard(1.0);
            var feet = new Feet(3.0);
            var inches = new Inches(36.0);

            Assert.IsTrue(yard.Equals(feet));
            Assert.IsTrue(feet.Equals(inches));
            Assert.IsTrue(yard.Equals(inches));
        }

        [Test]
        public void testEquality_AllUnits_ComplexScenario()
        {
            var yard = new Yard(2.0);
            var feet = new Feet(6.0);
            var inches = new Inches(72.0);

            Assert.IsTrue(yard.Equals(feet));
            Assert.IsTrue(feet.Equals(inches));
            Assert.IsTrue(yard.Equals(inches));
        }

        // -----------------------------------------------------------
        // Validation & Edge Cases
        // -----------------------------------------------------------

        [Test]
        public void testEquality_YardSameReference()
        {
            var yard = new Yard(5.0);

            Assert.IsTrue(yard.Equals(yard));
        }

        [Test]
        public void testEquality_YardNullComparison()
        {
            var yard = new Yard(5.0);

            Assert.IsFalse(yard.Equals(null));
        }

        [Test]
        public void testEquality_CentimetersSameReference()
        {
            var cm = new Centimeter(10.0);

            Assert.IsTrue(cm.Equals(cm));
        }

        [Test]
        public void testEquality_CentimetersNullComparison()
        {
            var cm = new Centimeter(10.0);

            Assert.IsFalse(cm.Equals(null));
        }

        [Test]
        public void testEquality_YardWithNullUnit()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var invalid = new CustomLengthWithNullUnit(1.0);
            });
        }

        [Test]
        public void testEquality_CentimetersWithNullUnit()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var invalid = new CustomLengthWithNullUnit(2.0);
            });
        }

        // -----------------------------------------------------------
        // Helper class to simulate null unit
        // -----------------------------------------------------------

        private class CustomLengthWithNullUnit : Length
        {
            public CustomLengthWithNullUnit(double value)
                : base(value, null!)
            {
            }
        }
    }
}