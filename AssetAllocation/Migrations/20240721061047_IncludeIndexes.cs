using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetAllocation.Api.Migrations
{
    /// <inheritdoc />
    public partial class IncludeIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mileages_Cars_CarId",
                table: "Mileages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mileages",
                table: "Mileages");

            migrationBuilder.RenameTable(
                name: "Mileages",
                newName: "CarMileages");

            migrationBuilder.RenameIndex(
                name: "IX_Mileages_CarId",
                table: "CarMileages",
                newName: "IX_CarMileages_CarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarMileages",
                table: "CarMileages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneBrands_Name",
                table: "PhoneBrands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_RegistrationNumber",
                table: "Persons",
                column: "RegistrationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarMakes_Name",
                table: "CarMakes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CarMileages_Cars_CarId",
                table: "CarMileages",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarMileages_Cars_CarId",
                table: "CarMileages");

            migrationBuilder.DropIndex(
                name: "IX_PhoneBrands_Name",
                table: "PhoneBrands");

            migrationBuilder.DropIndex(
                name: "IX_Persons_RegistrationNumber",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_CarMakes_Name",
                table: "CarMakes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CarMileages",
                table: "CarMileages");

            migrationBuilder.RenameTable(
                name: "CarMileages",
                newName: "Mileages");

            migrationBuilder.RenameIndex(
                name: "IX_CarMileages_CarId",
                table: "Mileages",
                newName: "IX_Mileages_CarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mileages",
                table: "Mileages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mileages_Cars_CarId",
                table: "Mileages",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
