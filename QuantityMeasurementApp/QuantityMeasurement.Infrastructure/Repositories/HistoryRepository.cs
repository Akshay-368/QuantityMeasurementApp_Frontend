using QuantityMeasurement.Infrastructure.Interfaces;
using QuantityMeasurement.ModelLayer.Models;
using QuantityMeasurement.Infrastructure.Persistence;
using QuantityMeasurement.ModelLayer.Entities;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace QuantityMeasurement.Infrastructure.Repositories;

public class HistoryRepository : IHistoryRepository
{
    private readonly QuantityDbContext _db;
    private readonly IDistributedCache _cache;
    private const string HistoryCacheKey = "history:all";

    private static readonly DistributedCacheEntryOptions CacheOptions = new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)  // Cache expiration set to 30 minutes
    };

    public HistoryRepository(QuantityDbContext db , IDistributedCache cache)
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

        // Invalidation of cache after write opertaion
        _cache.Remove(HistoryCacheKey);
    }

    public List<HistoryRecord> GetHistory()
    {
        // Try first to get from cache
        var cachedHistory = _cache.Get(HistoryCacheKey);
        if (cachedHistory != null)
        {
            var cachedJson = Encoding.UTF8.GetString(cachedHistory);
            try
            {
                var cachedList = JsonSerializer.Deserialize<List<HistoryRecord>>(cachedJson);
                if (cachedList != null) return cachedList;
            }
            catch
            {
                // Failed to deserialize : fall through To Db read and refresh  cache
            }
        }
        // Cached not found, read from Db and refresh cache
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
        
        // Also store it in cache for future use
        var json = JsonSerializer.Serialize(result);
        _cache.Set(HistoryCacheKey , Encoding.UTF8.GetBytes(json), CacheOptions);

        return result; // after saving it in cache safely now return it
    }

    public void ClearHistory()
    {
        // As table is small, remove all rows
        _db.Histories.RemoveRange(_db.Histories);
        _db.SaveChanges();

        // Also invalidate or clear the cache as well
        _cache.Remove(HistoryCacheKey);
    }
}