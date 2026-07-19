using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventSourcing.Bank.Modules.Email.Migrations
{
    /// <inheritdoc />
    public partial class InitialEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Email");

            migrationBuilder.CreateTable(
                name: "InboxMessages",
                schema: "Email",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InboxMessages",
                schema: "Email");
        }
    }
}
