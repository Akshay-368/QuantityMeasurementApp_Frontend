namespace QuantityMeasurement.AuthService.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuantityMeasurement.AuthService.Entities;
public interface IQuantityDbContext
{
     DbSet<History> Histories {get ; set ; }
     DbSet<User> Users { get; set; }

     int SaveChanges();
}