using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VrRetreat.WebApp.Data.Migrations
{
    public partial class AddMissingCooldownFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsernameCheck",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUsernameCheck",
                table: "AspNetUsers");
        }
    }
}
