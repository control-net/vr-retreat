using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VrRetreat.WebApp.Data.Migrations
{
    public partial class AddVerificationProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BioCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsVrChatAccountLinked",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastBioCheck",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastFriendRequestSent",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BioCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsVrChatAccountLinked",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastBioCheck",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastFriendRequestSent",
                table: "AspNetUsers");
        }
    }
}
