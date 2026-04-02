namespace QuantityMeasurement.Infrastructure.Interfaces;

using QuantityMeasurement.ModelLayer.Models;

public interface IHistoryRepository
{
    Task SaveAsync(HistoryRecord history);

    Task<List<HistoryRecord>> GetHistoryAsync();

    Task ClearHistoryAsync();
}