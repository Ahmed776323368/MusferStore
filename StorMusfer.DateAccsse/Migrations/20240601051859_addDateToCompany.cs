using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StoreMusfer.DateAccsse.Migrations
{
    /// <inheritdoc />
    public partial class addDateToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "companies",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAdress" },
                values: new object[,]
                {
                    { 1, "ffff", "Action", "776323368", "hhh", "il", "nbbb" },
                    { 2, "ffff", "hahab", "776323368", "hhh", "il", "nbbb" },
                    { 3, "ffff", "ahmed", "776323368", "hhh", "il", "nbbb" },
                    { 4, "ffff", "Actnmmm", "776323368", "hhh", "il", "nbbb" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
