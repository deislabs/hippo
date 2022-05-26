using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippo.Infrastructure.Data.Migrations.Postgresql
{
    public partial class AddRedisComponentElements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Route",
                table: "RevisionComponents",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "RevisionComponents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "RevisionComponents",
                type: "text",
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
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
