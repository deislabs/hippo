using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hippo.Migrations.Postgres
{
    public partial class DomainOwnedByChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Domains_DomainId",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Channels_DomainId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "Channels");

            migrationBuilder.AddColumn<Guid>(
                name: "ChannelId",
                table: "Domains",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Domains_ChannelId",
                table: "Domains",
                column: "ChannelId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Domains_Channels_ChannelId",
                table: "Domains",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Domains_Channels_ChannelId",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Domains_ChannelId",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Domains");

            migrationBuilder.AddColumn<Guid>(
                name: "DomainId",
                table: "Channels",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_DomainId",
                table: "Channels",
                column: "DomainId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Domains_DomainId",
                table: "Channels",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
