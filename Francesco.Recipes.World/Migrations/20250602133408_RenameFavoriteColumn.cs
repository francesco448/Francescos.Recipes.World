using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Francesco.Recipes.World.Migrations
{
    /// <inheritdoc />
    public partial class RenameFavoriteColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Favorits_FavoritId",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "FavoritId",
                table: "Recipes",
                newName: "FavoriteId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_FavoritId",
                table: "Recipes",
                newName: "IX_Recipes_FavoriteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Favorits_FavoriteId",
                table: "Recipes",
                column: "FavoriteId",
                principalTable: "Favorits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Favorits_FavoriteId",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "FavoriteId",
                table: "Recipes",
                newName: "FavoritId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_FavoriteId",
                table: "Recipes",
                newName: "IX_Recipes_FavoritId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Favorits_FavoritId",
                table: "Recipes",
                column: "FavoritId",
                principalTable: "Favorits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
