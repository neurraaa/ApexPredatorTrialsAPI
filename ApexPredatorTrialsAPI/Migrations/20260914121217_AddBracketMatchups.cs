using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApexPredatorTrialsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBracketMatchups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "CurrentRound",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartingRound",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "CurrentRound",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "StartingRound",
                table: "Events");
        }
    }
}
