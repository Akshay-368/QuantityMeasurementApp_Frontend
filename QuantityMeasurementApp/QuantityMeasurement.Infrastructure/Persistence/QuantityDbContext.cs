using Microsoft.EntityFrameworkCore;
using QuantityMeasurement.ModelLayer.Entities;
using QuantityMeasurement.Infrastructure.Interfaces;
namespace QuantityMeasurement.Infrastructure.Persistence;

public class QuantityDbContext : DbContext , IQuantityDbContext
{
    public QuantityDbContext(DbContextOptions<QuantityDbContext> options)
        : base(options)
    {
    }

    public DbSet<History> Histories { get; set; }

    public DbSet<User> Users { get; set; }

    /* This is totally optional and not needed at all as the class QuantityDbContext already inherits from DbContext.
    public override int SaveChanges()
    {
        return base.SaveChanges(); // calling base when overriding
    } 
    // public override int SaveChanges() => SaveChanges(); this is a self-call loop of doom 
    // as in the authservices it is like _db.SaveChanges(); so when calling the method
    // it will call my method since the class overrides it, and my version calls the same method again
    // thus self loop , or recursion and leads to stack overflow
    // thus it's always good to use base keyword which means call the base ( parent ) class method version of dbcontext
    */
}