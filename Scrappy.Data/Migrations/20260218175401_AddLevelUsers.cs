using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scrappy.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LevelUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GuildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    UserId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TotalXp = table.Column<long>(type: "bigint", nullable: false),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LevelUsers_GuildId_UserId",
                table: "LevelUsers",
                columns: new[] { "GuildId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LevelUsers");
        }
    }
}
