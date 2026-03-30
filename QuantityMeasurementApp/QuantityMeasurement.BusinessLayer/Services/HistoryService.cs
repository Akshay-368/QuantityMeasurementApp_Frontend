using QuantityMeasurement.ModelLayer.DTOs;
using QuantityMeasurement.BusinessLayer.Interfaces;
using QuantityMeasurement.Infrastructure.Interfaces;

namespace QuantityMeasurement.BusinessLayer.Services;

public class HistoryService : IHistoryService
{
    private readonly IHistoryRepository _historyRepository;

    public HistoryService(IHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }

    public List<HistoryDto> GetHistory()
    {
        return _historyRepository.GetHistory()
            .Select(h => new HistoryDto
            {
                Id = h.Id,
                Operation = h.Operation,
                Value1 = h.Value1,
                Unit1 = h.Unit1,
                Value2 = h.Value2,
                Unit2 = h.Unit2,
                TargetUnit = h.TargetUnit,
                Scalar = h.Scalar,
                Result = h.Result,
                ResultUnit = h.ResultUnit,
                CreatedAt = h.CreatedAt
            })
            .ToList();
    }

    public void ClearHistory()
    {
        _historyRepository.ClearHistory();
    }
}
