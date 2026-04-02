namespace QuantityMeasurement.ModelLayer.Core;
public abstract class Quantity : IQuantity<Quantity>
{
    // Implementing the IQuantity interface properties here
    public double value { get; }
    public Unit unit { get; }
    protected Quantity (double value, Unit unit)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
             throw new ArgumentException("Value must be a finite number.", nameof(value));
             
        this.value = value;
        this.unit = unit ?? throw new ArgumentNullException(nameof(unit)) ;
    }

    //  Force children to provide concrete instance creation
    public abstract Quantity CreateInstance(double value, Unit unit);

    protected double ToBase()
    {
        return unit.ConvertToBaseUnit(value);
    }


    /// <summary>
    /// UC1 Requirement: Handles Reflexive, Null, Type, and Value equality.
    /// </summary>
    public  override bool Equals (object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null) return false;

        // Only allow comparison between Quantity types
        if (obj is not Quantity other) return false;
        
        //  Enforce same category
        if (this.unit.Category != other.unit.Category) return false;

        double thisBase = this.ToBase();
        double otherBase = other.ToBase();
        return Math.Abs(thisBase - otherBase) < 0.0001;
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(value, unit);
    }

    public override string ToString()
    {
        return $"{Math.Round(value, 4)} {unit}";
    }

    public Quantity ConvertTo(Unit targetUnit) 
    {
        if (targetUnit is null) throw new ArgumentNullException(nameof(targetUnit)); 
        if (this.unit.Category != targetUnit.Category) throw new InvalidOperationException("Cannot convert between different unit categories."); 
        double baseValue = this.ToBase();
        double converted = targetUnit.ConvertFromBaseUnit(baseValue); 
        return CreateInstance(converted, targetUnit); 
    }

    public Quantity Add(Quantity other)
    { 
        return Add(other, this.unit); 
    }

    public Quantity Add(Quantity other, Unit targetUnit) 
    { 
        if (other is null) throw new ArgumentNullException(nameof(other));
        if (targetUnit is null) throw new ArgumentNullException(nameof(targetUnit)); 
        if (this.unit.Category != other.unit.Category) throw new InvalidOperationException("Cannot add different unit categories."); 
        double sumInBase = this.ToBase() + other.ToBase();
        double result = targetUnit.ConvertFromBaseUnit(sumInBase); 
        return CreateInstance(result, targetUnit); 
    }

    // --------------------------
    // Subtract (default to this unit)
    // --------------------------
    public Quantity Subtract(Quantity other)
    {
        return Subtract(other, this.unit);
    }

    // --------------------------
    // Subtract with target unit
    // --------------------------
    public Quantity Subtract(Quantity other, Unit targetUnit)
    {
        if (other is null) 
            throw new ArgumentNullException(nameof(other));

        if (targetUnit is null) 
            throw new ArgumentNullException(nameof(targetUnit));

        if (this.unit.Category != other.unit.Category) 
            throw new InvalidOperationException("Cannot subtract different unit categories.");

        double differenceInBase = this.ToBase() - other.ToBase();

        double result = targetUnit.ConvertFromBaseUnit(differenceInBase);

        return CreateInstance(result, targetUnit);
    }

    // --------------------------
    // Divide by another quantity
    // Returns scalar
    // --------------------------
    public double Divide(Quantity other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        if (this.unit.Category != other.unit.Category)
            throw new InvalidOperationException("Cannot divide different unit categories.");

        double thisBase = this.ToBase();
        double otherBase = other.ToBase();

        if (Math.Abs(otherBase) < 0.0000001)
            throw new DivideByZeroException("Cannot divide by zero quantity.");

        return thisBase / otherBase;
    }

    // --------------------------
    // Divide by scalar
    // --------------------------
    public Quantity Divide(double divisor)
    {
        if (Math.Abs(divisor) < 0.0000001)
            throw new DivideByZeroException("Cannot divide by zero.");

        double result = this.value / divisor;

        return CreateInstance(result, this.unit);
    }
}