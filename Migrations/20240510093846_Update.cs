using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyChas_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActiveStories_Characters_CharacterId",
                table: "ActiveStories");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Professions_ProfessionId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Species_SpeciesId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedStories_Characters_CharacterId",
                table: "SavedStories");

            migrationBuilder.DropTable(
                name: "Professions");

            migrationBuilder.DropTable(
                name: "Species");

            migrationBuilder.DropIndex(
                name: "IX_SavedStories_CharacterId",
                table: "SavedStories");

            migrationBuilder.DropIndex(
                name: "IX_ActiveStories_CharacterId",
                table: "ActiveStories");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "SavedStories");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "ActiveStories");

            migrationBuilder.RenameColumn(
                name: "SpeciesId",
                table: "Characters",
                newName: "SavedStoryId");

            migrationBuilder.RenameColumn(
                name: "ProfessionId",
                table: "Characters",
                newName: "ActiveStoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_SpeciesId",
                table: "Characters",
                newName: "IX_Characters_SavedStoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_ProfessionId",
                table: "Characters",
                newName: "IX_Characters_ActiveStoryId");

            migrationBuilder.AddColumn<bool>(
                name: "Favourite",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Profession",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Species",
                table: "Characters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_ActiveStories_ActiveStoryId",
                table: "Characters",
                column: "ActiveStoryId",
                principalTable: "ActiveStories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_SavedStories_SavedStoryId",
                table: "Characters",
                column: "SavedStoryId",
                principalTable: "SavedStories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_ActiveStories_ActiveStoryId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_SavedStories_SavedStoryId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Favourite",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Profession",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Species",
                table: "Characters");

            migrationBuilder.RenameColumn(
                name: "SavedStoryId",
                table: "Characters",
                newName: "SpeciesId");

            migrationBuilder.RenameColumn(
                name: "ActiveStoryId",
                table: "Characters",
                newName: "ProfessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_SavedStoryId",
                table: "Characters",
                newName: "IX_Characters_SpeciesId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_ActiveStoryId",
                table: "Characters",
                newName: "IX_Characters_ProfessionId");

            migrationBuilder.AddColumn<int>(
                name: "CharacterId",
                table: "SavedStories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CharacterId",
                table: "ActiveStories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Professions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpeciesName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedStories_CharacterId",
                table: "SavedStories",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveStories_CharacterId",
                table: "ActiveStories",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActiveStories_Characters_CharacterId",
                table: "ActiveStories",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Professions_ProfessionId",
                table: "Characters",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Species_SpeciesId",
                table: "Characters",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedStories_Characters_CharacterId",
                table: "SavedStories",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
