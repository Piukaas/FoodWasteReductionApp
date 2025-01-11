using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodWasteReduction.Infrastructure.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddCanteedIdToStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "CanteenStaff");

            migrationBuilder.AddColumn<int>(
                name: "CanteenId",
                table: "CanteenStaff",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanteenStaff_CanteenId",
                table: "CanteenStaff",
                column: "CanteenId");

            migrationBuilder.AddForeignKey(
                name: "FK_CanteenStaff_Canteens_CanteenId",
                table: "CanteenStaff",
                column: "CanteenId",
                principalTable: "Canteens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CanteenStaff_Canteens_CanteenId",
                table: "CanteenStaff");

            migrationBuilder.DropIndex(
                name: "IX_CanteenStaff_CanteenId",
                table: "CanteenStaff");

            migrationBuilder.DropColumn(
                name: "CanteenId",
                table: "CanteenStaff");

            migrationBuilder.AddColumn<int>(
                name: "Location",
                table: "CanteenStaff",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
