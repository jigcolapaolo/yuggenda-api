using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yuggenda.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnsureSingleBusinessOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BusinessMembers_BusinessId",
                table: "BusinessMembers",
                column: "BusinessId",
                unique: true,
                filter: "\"Role\" = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BusinessMembers_BusinessId",
                table: "BusinessMembers");
        }
    }
}
