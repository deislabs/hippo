using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippo.Infrastructure.Data.Migrations.Postgresql
{
    public partial class AddChannelDesiredStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DesiredStatus",
                table: "Channels",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredStatus",
                table: "Channels");
        }
    }
}
