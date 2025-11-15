using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BssRms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeReferencesToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderTakenById",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderedById",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderedById",
                table: "Order",
                column: "OrderedById");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderTakenById",
                table: "Order",
                column: "OrderTakenById");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Employee_OrderTakenById",
                table: "Order",
                column: "OrderTakenById",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Employee_OrderedById",
                table: "Order",
                column: "OrderedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Employee_OrderTakenById",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Employee_OrderedById",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_OrderedById",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_OrderTakenById",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderTakenById",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderedById",
                table: "Order");
        }
    }
}
