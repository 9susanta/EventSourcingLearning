using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventSourcing.Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class snapshottableAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankAccountSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    AggregateType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccountSnapshots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountSnapshots_AggregateId_Version",
                table: "BankAccountSnapshots",
                columns: new[] { "AggregateId", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankAccountSnapshots");
        }
    }
}
