using Microsoft.EntityFrameworkCore;
using QuantityMeasurement.HistoryService.Entities;

namespace QuantityMeasurement.HistoryService.Persistence;

public class QuantityDbContext : DbContext
{
    public QuantityDbContext(DbContextOptions<QuantityDbContext> options)
        : base(options)
    {
    }

    public DbSet<History> Histories { get; set; }
}
