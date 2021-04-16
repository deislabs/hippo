using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace hippo.Migrations
{
    public partial class AddConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Configuration_Applications_AppId",
                        column: x => x.AppId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EnvironmentVariable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    ConfigId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentVariable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvironmentVariable_Configuration_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "Configuration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_AppId",
                table: "Configuration",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentVariable_ConfigId",
                table: "EnvironmentVariable",
                column: "ConfigId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnvironmentVariable");

            migrationBuilder.DropTable(
                name: "Configuration");
        }
    }
}
