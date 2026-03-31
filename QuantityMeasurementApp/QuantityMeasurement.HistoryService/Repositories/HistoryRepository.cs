using Microsoft.Extensions.Caching.Distributed;
using QuantityMeasurement.HistoryService.Entities;
using QuantityMeasurement.HistoryService.Interfaces;
using QuantityMeasurement.HistoryService.Models;
using QuantityMeasurement.HistoryService.Persistence;
using System.Text;
using System.Text.Json;

namespace QuantityMeasurement.HistoryService.Repositories;

public class HistoryRepository : IHistoryRepository
{
    private readonly QuantityDbContext _db;
    private readonly IDistributedCache _cache;
    private const string HistoryCacheKey = "history:all";

    private static readonly DistributedCacheEntryOptions CacheOptions = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
    };

    public HistoryRepository(QuantityDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public void Save(HistoryRecord historyRecord)
    {
        var entity = new History
        {
            Id = historyRecord.Id == Guid.Empty ? Guid.NewGuid() : historyRecord.Id,
            Operation = historyRecord.Operation,
            Value1 = historyRecord.Value1,
            Unit1 = historyRecord.Unit1,
            Value2 = historyRecord.Value2,
            Unit2 = historyRecord.Unit2,
            TargetUnit = historyRecord.TargetUnit,
            Scalar = historyRecord.Scalar,
            Result = historyRecord.Result,
            ResultUnit = historyRecord.ResultUnit,
            CreatedAt = historyRecord.CreatedAt == default ? DateTime.UtcNow : historyRecord.CreatedAt
        };

        _db.Histories.Add(entity);
        _db.SaveChanges();

        _cache.Remove(HistoryCacheKey);
    }

    public List<HistoryRecord> GetHistory()
    {
        var cachedHistory = _cache.Get(HistoryCacheKey);
        if (cachedHistory != null)
        {
            var cachedJson = Encoding.UTF8.GetString(cachedHistory);
            try
            {
                var cachedList = JsonSerializer.Deserialize<List<HistoryRecord>>(cachedJson);
                if (cachedList != null)
                {
                    return cachedList;
                }
            }
            catch
            {
            }
        }

        var result = _db.Histories
            .OrderByDescending(h => h.CreatedAt)
            .Select(h => new HistoryRecord
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

        var json = JsonSerializer.Serialize(result);
        _cache.Set(HistoryCacheKey, Encoding.UTF8.GetBytes(json), CacheOptions);

        return result;
    }

    public void ClearHistory()
    {
        _db.Histories.RemoveRange(_db.Histories);
        _db.SaveChanges();

        _cache.Remove(HistoryCacheKey);
    }
}
