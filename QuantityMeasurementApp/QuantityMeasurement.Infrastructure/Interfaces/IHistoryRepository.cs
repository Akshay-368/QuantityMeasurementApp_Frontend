namespace QuantityMeasurement.Infrastructure.Interfaces;

using QuantityMeasurement.ModelLayer.Models;

public interface IHistoryRepository
{
    void Save(HistoryRecord history);

    List<HistoryRecord> GetHistory();

    void ClearHistory();
}