using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spenses.Resources.Relational.Migrations
{
    /// <inheritdoc />
    public partial class MandatoryExpenseCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expense_ExpenseCategory_CategoryId",
                table: "Expense");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "Expense",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_ExpenseCategory_CategoryId",
                table: "Expense",
                column: "CategoryId",
                principalTable: "ExpenseCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expense_ExpenseCategory_CategoryId",
                table: "Expense");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "Expense",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_ExpenseCategory_CategoryId",
                table: "Expense",
                column: "CategoryId",
                principalTable: "ExpenseCategory",
                principalColumn: "Id");
        }
    }
}
