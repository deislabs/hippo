using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace hippo.Migrations
{
    public partial class AddRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Applications_AppId",
                table: "Builds");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_AspNetUsers_OwnerId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Configuration_Applications_AppId",
                table: "Configuration");

            migrationBuilder.DropForeignKey(
                name: "FK_Domains_AspNetUsers_OwnerId",
                table: "Domains");

            migrationBuilder.DropForeignKey(
                name: "FK_Keys_AspNetUsers_OwnerId",
                table: "Keys");

            migrationBuilder.DropIndex(
                name: "IX_Keys_OwnerId",
                table: "Keys");

            migrationBuilder.DropIndex(
                name: "IX_Domains_OwnerId",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Configuration_AppId",
                table: "Configuration");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_OwnerId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_Builds_AppId",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Keys");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "Configuration");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "Builds");

            migrationBuilder.AddColumn<Guid>(
                name: "AppId",
                table: "Releases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AppId",
                table: "Domains",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DomainId",
                table: "Certificates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SigningKeyId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Releases_AppId",
                table: "Releases",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_AppId",
                table: "Domains",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_DomainId",
                table: "Certificates",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SigningKeyId",
                table: "AspNetUsers",
                column: "SigningKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Keys_SigningKeyId",
                table: "AspNetUsers",
                column: "SigningKeyId",
                principalTable: "Keys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Domains_DomainId",
                table: "Certificates",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Domains_Applications_AppId",
                table: "Domains",
                column: "AppId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Releases_Applications_AppId",
                table: "Releases",
                column: "AppId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Keys_SigningKeyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Domains_DomainId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Domains_Applications_AppId",
                table: "Domains");

            migrationBuilder.DropForeignKey(
                name: "FK_Releases_Applications_AppId",
                table: "Releases");

            migrationBuilder.DropIndex(
                name: "IX_Releases_AppId",
                table: "Releases");

            migrationBuilder.DropIndex(
                name: "IX_Domains_AppId",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_DomainId",
                table: "Certificates");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SigningKeyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "SigningKeyId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Keys",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Domains",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AppId",
                table: "Configuration",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Certificates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AppId",
                table: "Builds",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Keys_OwnerId",
                table: "Keys",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_OwnerId",
                table: "Domains",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_AppId",
                table: "Configuration",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_OwnerId",
                table: "Certificates",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Builds_AppId",
                table: "Builds",
                column: "AppId");

            migrationBuilder.AddForeignKey(
                name: "FK_Builds_Applications_AppId",
                table: "Builds",
                column: "AppId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_AspNetUsers_OwnerId",
                table: "Certificates",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Configuration_Applications_AppId",
                table: "Configuration",
                column: "AppId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Domains_AspNetUsers_OwnerId",
                table: "Domains",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Keys_AspNetUsers_OwnerId",
                table: "Keys",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
