using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippo.Infrastructure.Data.Migrations.Sqlite
{
    public partial class AddRedisComponentElements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Route",
                table: "RevisionComponents",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "RevisionComponents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "RevisionComponents",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "RevisionComponents");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "RevisionComponents");

            migrationBuilder.AlterColumn<string>(
                name: "Route",
                table: "RevisionComponents",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
