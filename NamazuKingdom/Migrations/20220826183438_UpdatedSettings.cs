using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NamazuKingdom.Migrations
{
    public partial class UpdatedSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShouldDisableSounds",
                table: "UserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShouldShowNickname",
                table: "UserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShouldShowPronouns",
                table: "UserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SoundVolumeLevel",
                table: "UserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShouldDisableSounds",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "ShouldShowNickname",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "ShouldShowPronouns",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "SoundVolumeLevel",
                table: "UserSettings");
        }
    }
}
