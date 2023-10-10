using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spenses.Resources.Relational.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Expense",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExpenseCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseCategory_Home_HomeId",
                        column: x => x.HomeId,
                        principalTable: "Home",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseCategory_UserIdentity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseCategory_UserIdentity_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expense_CategoryId",
                table: "Expense",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategory_CreatedById",
                table: "ExpenseCategory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategory_HomeId",
                table: "ExpenseCategory",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategory_ModifiedById",
                table: "ExpenseCategory",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_ExpenseCategory_CategoryId",
                table: "Expense",
                column: "CategoryId",
                principalTable: "ExpenseCategory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expense_ExpenseCategory_CategoryId",
                table: "Expense");

            migrationBuilder.DropTable(
                name: "ExpenseCategory");

            migrationBuilder.DropIndex(
                name: "IX_Expense_CategoryId",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Expense");
        }
    }
}
