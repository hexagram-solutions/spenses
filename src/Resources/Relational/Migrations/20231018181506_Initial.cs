using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spenses.Resources.Relational.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserIdentity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Home",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpensePeriod = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Home", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Home_UserIdentity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Home_UserIdentity_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultSplitPercentage = table.Column<double>(type: "float", nullable: false),
                    HomeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Member_Home_HomeId",
                        column: x => x.HomeId,
                        principalTable: "Home",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Member_UserIdentity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Member_UserIdentity_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Member_UserIdentity_UserId",
                        column: x => x.UserId,
                        principalTable: "UserIdentity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    PaidByMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HomeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expense_ExpenseCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ExpenseCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expense_Home_HomeId",
                        column: x => x.HomeId,
                        principalTable: "Home",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expense_Member_PaidByMemberId",
                        column: x => x.PaidByMemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expense_UserIdentity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expense_UserIdentity_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaidByMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HomeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Home_HomeId",
                        column: x => x.HomeId,
                        principalTable: "Home",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_Member_PaidByMemberId",
                        column: x => x.PaidByMemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_UserIdentity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_UserIdentity_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Expense_CategoryId",
                table: "Expense",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_CreatedById",
                table: "Expense",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_HomeId",
                table: "Expense",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_ModifiedById",
                table: "Expense",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_PaidByMemberId",
                table: "Expense",
                column: "PaidByMemberId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTag_ExpenseId",
                table: "ExpenseTag",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Home_CreatedById",
                table: "Home",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Home_ModifiedById",
                table: "Home",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Member_CreatedById",
                table: "Member",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Member_HomeId",
                table: "Member",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_ModifiedById",
                table: "Member",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Member_UserId",
                table: "Member",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_CreatedById",
                table: "Payment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_HomeId",
                table: "Payment",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ModifiedById",
                table: "Payment",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaidByMemberId",
                table: "Payment",
                column: "PaidByMemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseTag");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Expense");

            migrationBuilder.DropTable(
                name: "ExpenseCategory");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Home");

            migrationBuilder.DropTable(
                name: "UserIdentity");
        }
    }
}
