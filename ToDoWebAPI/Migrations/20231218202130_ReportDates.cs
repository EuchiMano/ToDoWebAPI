using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ReportDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "CompletedDuration",
                table: "Todos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Info_BadgePath",
                table: "Todos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Info_ReportPath",
                table: "Todos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDuration",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "Info_BadgePath",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "Info_ReportPath",
                table: "Todos");
        }
    }
}
