namespace QuantityMeasurement.QuantityService.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuantityMeasurement.QuantityService.Entities;
public interface IQuantityDbContext
{
     DbSet<History> Histories {get ; set ; }
     DbSet<User> Users { get; set; }

     int SaveChanges();
}