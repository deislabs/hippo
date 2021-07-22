using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hippo.Migrations.Postgres
{
    public partial class Collaborations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Applications_ApplicationId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ApplicationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "collaborations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collaborations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_collaborations_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_collaborations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_collaborations_ApplicationId",
                table: "collaborations",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_collaborations_UserId",
                table: "collaborations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "collaborations");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ApplicationId",
                table: "AspNetUsers",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Applications_ApplicationId",
                table: "AspNetUsers",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
