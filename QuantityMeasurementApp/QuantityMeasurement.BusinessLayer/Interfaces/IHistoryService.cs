using QuantityMeasurement.ModelLayer.DTOs;

namespace QuantityMeasurement.BusinessLayer.Interfaces;

public interface IHistoryService
{
    List<HistoryDto> GetHistory();
    void ClearHistory();
}
