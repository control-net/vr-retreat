using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VrRetreat.WebApp.Data.Migrations
{
    public partial class IsParticipatingProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsParticipating",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsParticipating",
                table: "AspNetUsers");
        }
    }
}
