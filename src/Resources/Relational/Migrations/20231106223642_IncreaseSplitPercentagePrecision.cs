using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spenses.Resources.Relational.Migrations;

/// <inheritdoc />
public partial class IncreaseSplitPercentagePrecision : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "DefaultSplitPercentage",
            table: "Member",
            type: "decimal(5,4)",
            precision: 5,
            scale: 4,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(3,2)",
            oldPrecision: 3,
            oldScale: 2);

        migrationBuilder.AlterColumn<decimal>(
            name: "OwedPercentage",
            table: "ExpenseShare",
            type: "decimal(5,4)",
            precision: 5,
            scale: 4,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(3,2)",
            oldPrecision: 3,
            oldScale: 2);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "DefaultSplitPercentage",
            table: "Member",
            type: "decimal(3,2)",
            precision: 3,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(5,4)",
            oldPrecision: 5,
            oldScale: 4);

        migrationBuilder.AlterColumn<decimal>(
            name: "OwedPercentage",
            table: "ExpenseShare",
            type: "decimal(3,2)",
            precision: 3,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(5,4)",
            oldPrecision: 5,
            oldScale: 4);
    }
}
