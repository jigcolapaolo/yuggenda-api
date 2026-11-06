using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yuggenda.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueBusinessUserCustomerIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_BusinessId",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_BusinessId_UserId",
                table: "Customers",
                columns: new[] { "BusinessId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_BusinessId_UserId",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_BusinessId",
                table: "Customers",
                column: "BusinessId");
        }
    }
}
