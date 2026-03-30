using Microsoft.EntityFrameworkCore.Migrations;
// To execute the  and update the db run this from inside infrastructure
// dotnet ef database update --startup-project ../QuantityMeasurement.API
// API project because it contains appsettings.json which contains the connection string
// and this line automatically creates the db if it doesn't exists and then applies all migrations
#nullable disable

namespace QuantityMeasurement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // To apply changes to the database ( forward )
            // to create tables , trigger and permissions etc
            migrationBuilder.Sql(@"
            CREATE TABLE SystemAudit (
                LogId INT IDENTITY(1,1) PRIMARY KEY,
                Id UNIQUEIDENTIFIER NULL,
                ActionType NVARCHAR(100) NOT NULL check (ActionType in ('Insert', 'Update', 'Delete', 'Unknown')),
                ActionDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
                OldValue NVARCHAR(MAX) NULL,
                NewValue NVARCHAR(MAX) NULL
            );
            ");

            migrationBuilder.Sql(@"Create Trigger trg_SystemAudit on Histories
            After Insert , Update , Delete
            As
            Begin
                Set NoCount On;
                Declare @ActionType NVarchar(100) ;
                If Exists (Select * from inserted) and Exists ( Select * from  deleted)
                     Set @ActionType = 'Update';
                Else If Exists (Select * from inserted)
                     Set @ActionType = 'Insert';
                Else If Exists (Select * from deleted)
                     Set @ActionType = 'Delete';
                Else
                     Set @ActionType = 'Unknown';
                Insert into SystemAudit (Id, ActionType, ActionDate, OldValue, NewValue)
                Select 
                     Coalesce ( i.ID , d.ID) ,
                     @ActionType ,
                     SYSUTCDATETIME() ,
                     (Select d.* For JSON PATH , WITHOUT_ARRAY_WRAPPER) ,
                     (Select i.* For JSON PATH , WITHOUT_ARRAY_WRAPPER)
                     from inserted i 
                     Full Outer Join deleted d on i.ID = d.ID ;
             End");

             migrationBuilder.Sql(" Deny Update , Delete on SystemAudit to public;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //so rollbacks actually work ( basically to undo changes )
            // to remove table table , trigger (permissions go away with table anyways)
            migrationBuilder.Sql("Drop Trigger IF EXISTS trg_SystemAudit;");
            migrationBuilder.Sql("Drop Table If Exists SystemAudit ;");
        }
    }
}
