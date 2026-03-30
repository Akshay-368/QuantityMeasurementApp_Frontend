using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
// dotnet add package Microsoft.Extensions.Configuration.Json
// dotnet add package Microsoft.Extensions.Configuration.FileExtensions
//Also run this line from inside infrastructure dotnet ef migrations add AddAuditTrigger --startup-project ../QuantityMeasurement.API

/*
So how does EF actually finds the models since i did not pass any path to models classes or files or folder or anything like that ?
Basically this file along with DbContext helps the EF in doing so.
As for EF my DbContext is the single source of truth and roadmap.
the steps internally goes like this :
EF first scans the entire project and finds the public class QuantityDbContext : DbContext
then it creates it , by going to the IDeignTimeDbContextFactory<QuantityDbContext> interface
and if this factory did not existed then EF would have tried program.cs  , which is not there in this project
and then reads DbSets<History> , DbSets<User> etc. and then scan the classes History and User for reading properties , types and relationships and constraints (if any)
and at last then it compares the current model ( from DbContext ) vs last snapshot file and take action based on the differences
*/

namespace QuantityMeasurement.Infrastructure.Persistence;

public class QuantityDbContextFactory : IDesignTimeDbContextFactory<QuantityDbContext>
{
    public QuantityDbContext CreateDbContext(string[] args)
    {
        // Go to API project to read appsettings.json
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../QuantityMeasurement.API");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<QuantityDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new QuantityDbContext(optionsBuilder.Options);
    }
}