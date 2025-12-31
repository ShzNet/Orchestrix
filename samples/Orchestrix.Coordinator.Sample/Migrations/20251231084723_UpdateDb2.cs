using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "CronSchedules");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "CoordinatorNodes");

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Workers",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Jobs",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "CronSchedules",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "CoordinatorNodes",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "CronSchedules");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "CoordinatorNodes");

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Workers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Jobs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "CronSchedules",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "CoordinatorNodes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
