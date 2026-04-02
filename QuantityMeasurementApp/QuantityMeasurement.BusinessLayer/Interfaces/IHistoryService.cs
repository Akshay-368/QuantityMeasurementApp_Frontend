using QuantityMeasurement.ModelLayer.DTOs;

namespace QuantityMeasurement.BusinessLayer.Interfaces;

public interface IHistoryService
{
    Task<List<HistoryDto>> GetHistoryAsync();
    Task ClearHistoryAsync();
}
