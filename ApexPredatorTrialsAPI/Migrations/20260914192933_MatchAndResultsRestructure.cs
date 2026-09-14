using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApexPredatorTrialsAPI.Migrations
{
    /// <inheritdoc />
    public partial class MatchAndResultsRestructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Players_HumanPlayerId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Players_HunterPlayerId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Players_WinnerPlayerId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_HumanPlayerId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_HunterPlayerId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_WinnerPlayerId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "HumanPlayerId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "HunterPlayerId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "WinnerPlayerId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "DateTimeConcluded",
                table: "MatchResults");

            migrationBuilder.InsertData(
                table: "Maps",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Old Town" },
                    { 2, "Slums" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Maps",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Maps",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "HumanPlayerId",
                table: "Schedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HunterPlayerId",
                table: "Schedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WinnerPlayerId",
                table: "Schedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeConcluded",
                table: "MatchResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_HumanPlayerId",
                table: "Schedules",
                column: "HumanPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_HunterPlayerId",
                table: "Schedules",
                column: "HunterPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_WinnerPlayerId",
                table: "Schedules",
                column: "WinnerPlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Players_HumanPlayerId",
                table: "Schedules",
                column: "HumanPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Players_HunterPlayerId",
                table: "Schedules",
                column: "HunterPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Players_WinnerPlayerId",
                table: "Schedules",
                column: "WinnerPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
