namespace QuantityMeasurement.ModelLayer.Entities;
using Microsoft.EntityFrameworkCore; // dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
// To add this to migrations , run dotnet ef migrations add AddUsers --startup-project ../QuantityMeasurement.API  in the Infrastructure project
// and then update the database to actually add the table users by running dotnet ef database update --startup-project ../QuantityMeasurement.API
// dotnet ef migrations add AddUsers compares the c# models with the last migration ( the c# instruction which generates sql) snapshot ( EF's memory of last known model state) and generates sql instructions and create files like AddUsers.cs , AddUsers.Designer.cs
// this records what changed and creates the migrations for it.
// the dotnet ef database update command only takes the existing migrations and executes them on db and does not create any new migrations thus would have totally missed the users table if the above commadn was not executed successfully first hand.
[Index(nameof(Username), IsUnique = true)]
/*
Also run these commands :
dotnet ef migrations add AddUserIndex --startup-project ../QuantityMeasurement.API
dotnet ef database update --startup-project ../QuantityMeasurement.API
because i am creating the index after table has been already created , so now it needs new migration and then taht migration should get updated to db.
*/
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;

    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
}