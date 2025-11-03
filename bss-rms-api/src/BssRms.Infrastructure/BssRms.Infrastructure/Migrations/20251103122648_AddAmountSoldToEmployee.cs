using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BssRms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAmountSoldToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountSold",
                table: "Employee",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountSold",
                table: "Employee");
        }
    }
}
