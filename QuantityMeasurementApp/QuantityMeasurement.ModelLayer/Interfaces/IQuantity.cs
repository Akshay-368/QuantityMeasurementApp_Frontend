namespace QuantityMeasurement.ModelLayer.Core;
using System;
/// <summary>
/// This is an interface for the quantity.
/// It requires a value and a unit to be set, which must be a double and an Unit respectively.
/// It also requires the Equals and GetHashCode methods.
/// </summary>
/// <typeparam name="T"></typeparam>
public  interface IQuantity<T>
{
    // Not using set to make sure that the value and the unit cannot be changed
    // Thus preserving Immutability.
    double value { get; }
    Unit unit { get; }
    // This way of only using get ensures the value and the unit are passed through the construtor and never tampered with, which is a key part of "Encapsulation"

    bool Equals(object obj); // TO enforce that any unit must implement its own equality logic
    int GetHashCode();
}