using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiPmaPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HourlyCostRate",
                table: "Users",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BillingRatePerHour",
                table: "Projects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Projects",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourlyCostRate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BillingRatePerHour",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Projects");
        }
    }
}
