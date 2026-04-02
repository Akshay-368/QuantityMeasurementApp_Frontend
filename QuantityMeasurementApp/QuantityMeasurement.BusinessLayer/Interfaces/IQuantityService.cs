namespace QuantityMeasurement.BusinessLayer.Interfaces;
using QuantityMeasurement.ModelLayer.DTOs;

public interface IQuantityService
{
    Task<QuantityResultDto> ConvertAsync(QuantityRequestDto request);

    Task<QuantityResultDto> AddAsync(QuantityRequestDto request);

    Task<QuantityResultDto> SubtractAsync(QuantityRequestDto request);

    Task<QuantityResultDto> DivideByScalarAsync(QuantityRequestDto request);

    Task<double> DivideByQuantityAsync(QuantityRequestDto request);
}
