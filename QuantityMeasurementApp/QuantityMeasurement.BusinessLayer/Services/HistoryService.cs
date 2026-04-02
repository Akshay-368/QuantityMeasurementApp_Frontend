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

    public async Task<List<HistoryDto>> GetHistoryAsync()
    {
        var history = await _historyRepository.GetHistoryAsync();

        return history
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

    public async Task ClearHistoryAsync()
    {
        await _historyRepository.ClearHistoryAsync();
    }
}
