using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoGamesReboot24.Migrations
{
    public partial class EnhancedProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Systems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Systems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    AgeRating = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LongImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<double>(type: "float", nullable: true),
                    RatingCount = table.Column<int>(type: "int", nullable: true),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoGameCategories",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    VideoGamesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGameCategories", x => new { x.CategoriesId, x.VideoGamesId });
                    table.ForeignKey(
                        name: "FK_VideoGameCategories_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoGameCategories_VideoGames_VideoGamesId",
                        column: x => x.VideoGamesId,
                        principalTable: "VideoGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoGameSystems",
                columns: table => new
                {
                    SystemsId = table.Column<int>(type: "int", nullable: false),
                    VideoGamesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGameSystems", x => new { x.SystemsId, x.VideoGamesId });
                    table.ForeignKey(
                        name: "FK_VideoGameSystems_Systems_SystemsId",
                        column: x => x.SystemsId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoGameSystems_VideoGames_VideoGamesId",
                        column: x => x.VideoGamesId,
                        principalTable: "VideoGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoGameCategories_VideoGamesId",
                table: "VideoGameCategories",
                column: "VideoGamesId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoGameSystems_VideoGamesId",
                table: "VideoGameSystems",
                column: "VideoGamesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoGameCategories");

            migrationBuilder.DropTable(
                name: "VideoGameSystems");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Systems");

            migrationBuilder.DropTable(
                name: "VideoGames");
        }
    }
}
