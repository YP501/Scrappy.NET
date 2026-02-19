using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scrappy.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLevelUsersLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentLevel",
                table: "LevelUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentLevel",
                table: "LevelUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
