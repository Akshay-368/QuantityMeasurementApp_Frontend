namespace QuantityMeasurement.QuantityService.Interfaces;
using QuantityMeasurement.QuantityService.DTOs;

public interface IQuantityService
{
    QuantityResultDto Convert(QuantityRequestDto request);

     QuantityResultDto Add(QuantityRequestDto request);

    QuantityResultDto Subtract(QuantityRequestDto request);

    QuantityResultDto DivideByScalar(QuantityRequestDto request);

    double DivideByQuantity(QuantityRequestDto request);
}
