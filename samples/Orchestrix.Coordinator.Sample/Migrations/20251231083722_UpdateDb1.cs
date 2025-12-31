using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Workers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "CronSchedules",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Hostname",
                table: "CoordinatorNodes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessId",
                table: "CoordinatorNodes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "CoordinatorNodes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "CronSchedules");

            migrationBuilder.DropColumn(
                name: "Hostname",
                table: "CoordinatorNodes");

            migrationBuilder.DropColumn(
                name: "ProcessId",
                table: "CoordinatorNodes");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "CoordinatorNodes");
        }
    }
}
