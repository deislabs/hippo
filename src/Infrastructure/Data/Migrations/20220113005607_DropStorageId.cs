using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippo.Infrastructure.Data.Migrations
{
    public partial class DropStorageId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "Apps");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageId",
                table: "Apps",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
