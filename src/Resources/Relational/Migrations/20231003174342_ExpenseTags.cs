using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spenses.Resources.Relational.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Expense",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ExpenseTag",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseTag", x => new { x.Name, x.ExpenseId });
                    table.ForeignKey(
                        name: "FK_ExpenseTag_Expense_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expense",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTag_ExpenseId",
                table: "ExpenseTag",
                column: "ExpenseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseTag");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Expense",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
