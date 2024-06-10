using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreMusfer.DateAccsse.Migrations
{
    /// <inheritdoc />
    public partial class addSessionIdToOrderHeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "orderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "orderHeaders");
        }
    }
}
