namespace QuantityMeasurement.BusinessLayer.Services;
using QuantityMeasurement.ModelLayer.DTOs;
using QuantityMeasurement.BusinessLayer.Interfaces;
using QuantityMeasurement.ModelLayer.Models;
using QuantityMeasurement.Infrastructure.Interfaces;
using QuantityMeasurement.ModelLayer.Core;
using QuantityMeasurement.ModelLayer.Interfaces;
using QuantityMeasurement.ModelLayer.Units;

/// <summary>
/// This service class is responsible for converting between different units of measurement.
/// Basically it acts as the bridge between API and Domain
/// </summary>
public class QuantityService : IQuantityService
{
    private readonly IHistoryRepository _historyRepository;

    public QuantityService(IHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }

    public QuantityResultDto Convert(QuantityRequestDto request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Unit1))
            throw new ArgumentException("Unit1 must be provided.", nameof(request.Unit1));
        if (string.IsNullOrWhiteSpace(request.TargetUnit))
            throw new ArgumentException("TargetUnit must be provided.", nameof(request.TargetUnit));

        Unit fromUnit = ResolveUnit(request.Unit1);
        Unit toUnit = ResolveUnit(request.TargetUnit);

        Quantity quantity = CreateQuantity(request.Value1, fromUnit);

        Quantity result = quantity.ConvertTo(toUnit);

        // Save history
        var record = new HistoryRecord
        {
            Id = Guid.NewGuid(),
            Operation = "Convert",
            Value1 = request.Value1,
            Unit1 = request.Unit1,
            TargetUnit = request.TargetUnit,
            Result = result.value,
            ResultUnit = result.unit.Name,
            CreatedAt = DateTime.UtcNow
        };

        _historyRepository.Save(record);

        return new QuantityResultDto
        {
            Result = result.value,
            Unit = result.unit.Name
        };
    }

    private Unit ResolveUnit(string unitName)
    {
        if (string.IsNullOrWhiteSpace(unitName) )
        throw new ArgumentException("Unit name cannot be null or empty.", nameof(unitName));

        return unitName.ToLower() switch
        {
            "feet" => Unit.Feet,
            "inch" => Unit.Inch,
            "yard" => Unit.Yard,
            "meter" => Unit.Meter,
            "centimeter" => Unit.Centimeter,

            "kilogram" => Unit.Kilogram,
            "gram" => Unit.Gram,
            "pound" => Unit.Pound,

            "litre" => Unit.Litre,
            "millilitre" => Unit.Millilitre,
            "gallon" => Unit.Gallon,

            "celsius" => Unit.Celsius,
            "fahrenheit" => Unit.Fahrenheit,
            "kelvin" => Unit.Kelvin,

            _ => throw new ArgumentException($"Unsupported unit {unitName}")
        };
    }

    private Quantity CreateQuantity(double value, Unit unit)
    {
        if (unit.Category == "Length")
        {
            if (unit == Unit.Feet) return new Feet(value);
            if (unit == Unit.Inch) return new Inches(value);
            if (unit == Unit.Yard) return new Yard(value);
            if (unit == Unit.Meter) return new Meter(value);
            if (unit == Unit.Centimeter) return new Centimeter(value);
        }

        if (unit.Category == "Weight")
        {
            if (unit == Unit.Kilogram) return new Kilogram(value);
            if (unit == Unit.Gram) return new Gram(value);
            if (unit == Unit.Pound) return new Pound(value);
        }

        if (unit.Category == "Volume")
        {
            if (unit == Unit.Litre) return new Litre(value);
            if (unit == Unit.Millilitre) return new Millilitre(value);
            if (unit == Unit.Gallon) return new Gallon(value);
        }

        if (unit.Category == "Temperature")
        {
            if (unit == Unit.Celsius) return new Celsius(value);
            if (unit == Unit.Fahrenheit) return new Fahrenheit(value);
            if (unit == Unit.Kelvin) return new Kelvin(value);
        }

        throw new InvalidOperationException("Unsupported quantity type.");
    }

    public QuantityResultDto Add(QuantityRequestDto request)
    {
        Quantity q1 = CreateQuantity(request.Value1, ResolveUnit(request.Unit1));
        Quantity q2 = CreateQuantity(request.Value2!.Value, ResolveUnit(request.Unit2!));

        Unit targetUnit = request.TargetUnit != null
            ? ResolveUnit(request.TargetUnit)
            : q1.unit;

        Quantity result = q1.Add(q2, targetUnit);

        var record = new HistoryRecord
        {
            Id = Guid.NewGuid(),
            Operation = "Add",
            Value1 = request.Value1,
            Unit1 = request.Unit1,
            Value2 = request.Value2,
            Unit2 = request.Unit2,
            TargetUnit = request.TargetUnit,
            Result = result.value,
            ResultUnit = result.unit.Name,
            CreatedAt = DateTime.UtcNow
        };

        _historyRepository.Save(record);

        return new QuantityResultDto
        {
            Result = result.value,
            Unit = result.unit.Name
        };
    }

    public QuantityResultDto Subtract(QuantityRequestDto request)
    {
        Quantity q1 = CreateQuantity(request.Value1, ResolveUnit(request.Unit1));
        Quantity q2 = CreateQuantity(request.Value2!.Value, ResolveUnit(request.Unit2!));

        Unit targetUnit = request.TargetUnit != null
            ? ResolveUnit(request.TargetUnit)
            : q1.unit;

        Quantity result = q1.Subtract(q2, targetUnit);

        var record = new HistoryRecord
        {
            Id = Guid.NewGuid(),
            Operation = "Subtract",
            Value1 = request.Value1,
            Unit1 = request.Unit1,
            Value2 = request.Value2,
            Unit2 = request.Unit2,
            TargetUnit = request.TargetUnit,
            Result = result.value,
            ResultUnit = result.unit.Name,
            CreatedAt = DateTime.UtcNow
        };

        _historyRepository.Save(record);

        return new QuantityResultDto
        {
            Result = result.value,
            Unit = result.unit.Name
        };
    }

    public QuantityResultDto DivideByScalar(QuantityRequestDto request)
    {
        Quantity q1 = CreateQuantity(request.Value1, ResolveUnit(request.Unit1));

        Quantity result = q1.Divide(request.Scalar!.Value);

        var record = new HistoryRecord
        {
            Id = Guid.NewGuid(),
            Operation = "DivideByScalar",
            Value1 = request.Value1,
            Unit1 = request.Unit1,
            Scalar = request.Scalar,
            Result = result.value,
            ResultUnit = result.unit.Name,
            CreatedAt = DateTime.UtcNow
        };

        _historyRepository.Save(record);

        return new QuantityResultDto
        {
            Result = result.value,
            Unit = result.unit.Name
        };
    }


    public double DivideByQuantity(QuantityRequestDto request)
    {
        Quantity q1 = CreateQuantity(request.Value1, ResolveUnit(request.Unit1));
        Quantity q2 = CreateQuantity(request.Value2!.Value, ResolveUnit(request.Unit2!));
        
        double resultScalar = q1.Divide(q2);

        var record = new HistoryRecord
        {
            Id = Guid.NewGuid(),
            Operation = "DivideByQuantity",
            Value1 = request.Value1,
            Unit1 = request.Unit1,
            Value2 = request.Value2,
            Unit2 = request.Unit2,
            Result = resultScalar,
            ResultUnit = string.Empty, // unitless
            CreatedAt = DateTime.UtcNow
        };

        _historyRepository.Save(record);

        return q1.Divide(q2);
    }

}