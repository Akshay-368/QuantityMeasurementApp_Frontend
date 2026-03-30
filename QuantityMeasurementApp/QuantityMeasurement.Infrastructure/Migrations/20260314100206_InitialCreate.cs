using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantityMeasurement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value1 = table.Column<double>(type: "float", nullable: false),
                    Unit1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value2 = table.Column<double>(type: "float", nullable: true),
                    Unit2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scalar = table.Column<double>(type: "float", nullable: true),
                    Result = table.Column<double>(type: "float", nullable: false),
                    ResultUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Histories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Histories");
        }
    }
}
