using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiPmaPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRaidItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RaidItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Impact = table.Column<int>(type: "int", nullable: false),
                    Probability = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    MitigationPlan = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RootCause = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsEscalated = table.Column<bool>(type: "bit", nullable: false),
                    EscalatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaidItems", x => x.Id);
                    table.CheckConstraint("CK_RaidItems_Impact", "[Impact] BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_RaidItems_Probability", "[Probability] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_RaidItems_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaidItems_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RaidItems_Users_ReportedById",
                        column: x => x.ReportedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaidItems_OwnerId",
                table: "RaidItems",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RaidItems_ProjectId",
                table: "RaidItems",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RaidItems_ReportedById",
                table: "RaidItems",
                column: "ReportedById");

            migrationBuilder.CreateIndex(
                name: "IX_RaidItems_Status",
                table: "RaidItems",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaidItems");
        }
    }
}
