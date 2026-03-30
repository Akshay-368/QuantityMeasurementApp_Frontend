using QuantityMeasurement.ModelLayer.Core;

namespace QuantityMeasurement.ModelLayer.Units;

/// <summary>
/// Abstract sub-base class for Volume quantities.
/// Inherits common logic from Quantity base class.
/// </summary>
public abstract class Volume : Quantity
{
    protected Volume(double value, Unit unit) : base(value, unit)
    {
    }

    // --------------------------
    // Convert
    // --------------------------
    public Volume ConvertTo(Unit targetUnit)
    {
        return (Volume)base.ConvertTo(targetUnit);
    }

    // --------------------------
    // Add (default to this unit)
    // --------------------------
    public Volume Add(Volume other)
    {
        return (Volume)base.Add(other);
    }

    // --------------------------
    // Add with target unit
    // --------------------------
    public Volume Add(Volume other, Unit targetUnit)
    {
        return (Volume)base.Add(other, targetUnit);
    }

    // --------------------------
    // Factory Method
    // --------------------------
    public override Quantity CreateInstance(double value, Unit unit)
    {
        if (unit == Unit.Litre) return new Litre(value);
        if (unit == Unit.Millilitre) return new Millilitre(value);
        if (unit == Unit.Gallon) return new Gallon(value);

        throw new InvalidOperationException("Unsupported volume unit.");
    }

    public Volume Subtract(Volume other)
    {
        return (Volume)base.Subtract(other);
    }

    public Volume Subtract(Volume other, Unit targetUnit)
    {
        return (Volume)base.Subtract(other, targetUnit);
    }

    public double Divide(Volume other)
    {
        return base.Divide(other);
    }

    public Volume Divide(double divisor)
    {
        return (Volume)base.Divide(divisor);
    }

}