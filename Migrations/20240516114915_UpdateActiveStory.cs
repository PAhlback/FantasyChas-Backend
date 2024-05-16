using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyChas_Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActiveStory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatHistories_ActiveStories_ActiveStoryId",
                table: "ChatHistories");

            migrationBuilder.DropIndex(
                name: "IX_ChatHistories_ActiveStoryId",
                table: "ChatHistories");

            migrationBuilder.DropColumn(
                name: "ActiveStoryId",
                table: "ChatHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveStoryId",
                table: "ChatHistories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatHistories_ActiveStoryId",
                table: "ChatHistories",
                column: "ActiveStoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatHistories_ActiveStories_ActiveStoryId",
                table: "ChatHistories",
                column: "ActiveStoryId",
                principalTable: "ActiveStories",
                principalColumn: "Id");
        }
    }
}
