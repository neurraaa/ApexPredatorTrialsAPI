using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApexPredatorTrialsAPI.Migrations
{
    /// <inheritdoc />
    public partial class DbChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatcheResults_Matches_MatchId",
                table: "MatcheResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MatcheResults_Players_LoserId",
                table: "MatcheResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MatcheResults_Players_WinnerId",
                table: "MatcheResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_PlayersStats_PlayerStatsId",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayersStats",
                table: "PlayersStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatcheResults",
                table: "MatcheResults");

            migrationBuilder.RenameTable(
                name: "PlayersStats",
                newName: "PlayersStatistics");

            migrationBuilder.RenameTable(
                name: "MatcheResults",
                newName: "MatchResults");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameIndex(
                name: "IX_MatcheResults_WinnerId",
                table: "MatchResults",
                newName: "IX_MatchResults_WinnerId");

            migrationBuilder.RenameIndex(
                name: "IX_MatcheResults_MatchId",
                table: "MatchResults",
                newName: "IX_MatchResults_MatchId");

            migrationBuilder.RenameIndex(
                name: "IX_MatcheResults_LoserId",
                table: "MatchResults",
                newName: "IX_MatchResults_LoserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayersStatistics",
                table: "PlayersStatistics",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchResults",
                table: "MatchResults",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchResults_Matches_MatchId",
                table: "MatchResults",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchResults_Players_LoserId",
                table: "MatchResults",
                column: "LoserId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchResults_Players_WinnerId",
                table: "MatchResults",
                column: "WinnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_PlayersStatistics_PlayerStatsId",
                table: "Players",
                column: "PlayerStatsId",
                principalTable: "PlayersStatistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchResults_Matches_MatchId",
                table: "MatchResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchResults_Players_LoserId",
                table: "MatchResults");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchResults_Players_WinnerId",
                table: "MatchResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_PlayersStatistics_PlayerStatsId",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayersStatistics",
                table: "PlayersStatistics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchResults",
                table: "MatchResults");

            migrationBuilder.RenameTable(
                name: "PlayersStatistics",
                newName: "PlayersStats");

            migrationBuilder.RenameTable(
                name: "MatchResults",
                newName: "MatcheResults");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Password");

            migrationBuilder.RenameIndex(
                name: "IX_MatchResults_WinnerId",
                table: "MatcheResults",
                newName: "IX_MatcheResults_WinnerId");

            migrationBuilder.RenameIndex(
                name: "IX_MatchResults_MatchId",
                table: "MatcheResults",
                newName: "IX_MatcheResults_MatchId");

            migrationBuilder.RenameIndex(
                name: "IX_MatchResults_LoserId",
                table: "MatcheResults",
                newName: "IX_MatcheResults_LoserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayersStats",
                table: "PlayersStats",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatcheResults",
                table: "MatcheResults",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatcheResults_Matches_MatchId",
                table: "MatcheResults",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatcheResults_Players_LoserId",
                table: "MatcheResults",
                column: "LoserId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MatcheResults_Players_WinnerId",
                table: "MatcheResults",
                column: "WinnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_PlayersStats_PlayerStatsId",
                table: "Players",
                column: "PlayerStatsId",
                principalTable: "PlayersStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
