using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.EF.Migrations
{
    /// <inheritdoc />

    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[,]
                {
                { Guid.NewGuid().ToString(), "User", "USER" },
                { Guid.NewGuid().ToString(), "TeamLeader", "TEAMLEADER" },
                { Guid.NewGuid().ToString(), "Administrator", "ADMINISTRATOR" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Name",
                keyValues: new[] { "User", "TeamLeader", "Administrator" });
        }
    }



}
