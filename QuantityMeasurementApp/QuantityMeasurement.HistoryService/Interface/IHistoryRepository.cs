using QuantityMeasurement.HistoryService.Models;

namespace QuantityMeasurement.HistoryService.Interfaces;

public interface IHistoryRepository
{
    void Save(HistoryRecord history);

    List<HistoryRecord> GetHistory();

    void ClearHistory();
}
