using QuantityMeasurement.ModelLayer.Core;

namespace QuantityMeasurement.ModelLayer.Units;

/// <summary>
/// Concrete Volume unit: Millilitre
/// </summary>
public class Millilitre : Volume
{
    public Millilitre(double value) : base(value, Unit.Millilitre)
    {
    }
}