using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hippo.Infrastructure.Data.Migrations
{
    public partial class AddCertificates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CertificateId",
                table: "Channels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    PublicKey = table.Column<string>(type: "TEXT", nullable: false),
                    PrivateKey = table.Column<string>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_CertificateId",
                table: "Channels",
                column: "CertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Certificates_CertificateId",
                table: "Channels",
                column: "CertificateId",
                principalTable: "Certificates",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Certificates_CertificateId",
                table: "Channels");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Channels_CertificateId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "CertificateId",
                table: "Channels");
        }
    }
}
