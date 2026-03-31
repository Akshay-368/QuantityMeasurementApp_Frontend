namespace QuantityMeasurement.QuantityService.Interfaces;

using QuantityMeasurement.QuantityService.Models;

public interface IHistoryRepository
{
    void Save(HistoryRecord history);

    List<HistoryRecord> GetHistory();

    void ClearHistory();
}