using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scrappy.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLevelUsersDataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "Id",
                table: "LevelUsers",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_LevelUsers_GuildConfigs_GuildId",
                table: "LevelUsers",
                column: "GuildId",
                principalTable: "GuildConfigs",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LevelUsers_GuildConfigs_GuildId",
                table: "LevelUsers");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "LevelUsers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
