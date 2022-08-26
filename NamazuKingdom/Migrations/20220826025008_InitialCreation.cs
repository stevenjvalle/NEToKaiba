using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NamazuKingdom.Migrations
{
    public partial class InitialCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DiscordUserId = table.Column<long>(type: "INTEGER", nullable: false),
                    Nickname = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PreferredPronouns = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Birthday = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserRefId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UseTTS = table.Column<bool>(type: "INTEGER", nullable: false),
                    TTSVoiceName = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    ShouldShowBirthday = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_DiscordUsers_UserRefId",
                        column: x => x.UserRefId,
                        principalTable: "DiscordUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserRefId",
                table: "UserSettings",
                column: "UserRefId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "DiscordUsers");
        }
    }
}
