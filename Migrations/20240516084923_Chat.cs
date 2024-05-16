using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyChas_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Chat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Answer",
                table: "ChatHistories");

            migrationBuilder.DropColumn(
                name: "Prompt",
                table: "ChatHistories");

            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "ChatHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "ChatHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatSummary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActiveStoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chats_ActiveStories_ActiveStoryId",
                        column: x => x.ActiveStoryId,
                        principalTable: "ActiveStories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatHistories_ChatId",
                table: "ChatHistories",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ActiveStoryId",
                table: "Chats",
                column: "ActiveStoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatHistories_Chats_ChatId",
                table: "ChatHistories",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatHistories_Chats_ChatId",
                table: "ChatHistories");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_ChatHistories_ChatId",
                table: "ChatHistories");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "ChatHistories");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "ChatHistories");

            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "ChatHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prompt",
                table: "ChatHistories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
