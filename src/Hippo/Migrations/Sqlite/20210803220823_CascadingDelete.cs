using Microsoft.EntityFrameworkCore.Migrations;

namespace Hippo.Migrations.Sqlite
{
    public partial class CascadingDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Applications_ApplicationId",
                table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_EnvironmentVariables_Configuration_ConfigurationId",
                table: "EnvironmentVariables");

            migrationBuilder.DropForeignKey(
                name: "FK_Revisions_Applications_ApplicationId",
                table: "Revisions");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Applications_ApplicationId",
                table: "Channels",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EnvironmentVariables_Configuration_ConfigurationId",
                table: "EnvironmentVariables",
                column: "ConfigurationId",
                principalTable: "Configuration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Revisions_Applications_ApplicationId",
                table: "Revisions",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Applications_ApplicationId",
                table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_EnvironmentVariables_Configuration_ConfigurationId",
                table: "EnvironmentVariables");

            migrationBuilder.DropForeignKey(
                name: "FK_Revisions_Applications_ApplicationId",
                table: "Revisions");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Applications_ApplicationId",
                table: "Channels",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EnvironmentVariables_Configuration_ConfigurationId",
                table: "EnvironmentVariables",
                column: "ConfigurationId",
                principalTable: "Configuration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Revisions_Applications_ApplicationId",
                table: "Revisions",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
