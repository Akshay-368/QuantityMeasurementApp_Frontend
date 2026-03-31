using QuantityMeasurement.HistoryService.DTOs;

namespace QuantityMeasurement.HistoryService.Interfaces;

public interface IHistoryService
{
    List<HistoryDto> GetHistory();
    void ClearHistory();
}
