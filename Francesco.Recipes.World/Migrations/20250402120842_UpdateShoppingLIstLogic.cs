using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Francesco.Recipes.World.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShoppingLIstLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredientsShoppingLists_ShoppingLists_ShoppingListId",
                table: "RecipeIngredientsShoppingLists");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Categories_CategoryId",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "ShoppingListId",
                table: "RecipeIngredientsShoppingLists",
                newName: "RecipeShoppingListId");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeIngredientsShoppingLists_ShoppingListId",
                table: "RecipeIngredientsShoppingLists",
                newName: "IX_RecipeIngredientsShoppingLists_RecipeShoppingListId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "RecipeIngredientsShoppingLists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RecipeShoppingLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShoppingListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeShoppingLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeShoppingLists_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeShoppingLists_ShoppingLists_ShoppingListId",
                        column: x => x.ShoppingListId,
                        principalTable: "ShoppingLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeShoppingLists_RecipeId",
                table: "RecipeShoppingLists",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeShoppingLists_ShoppingListId",
                table: "RecipeShoppingLists",
                column: "ShoppingListId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredientsShoppingLists_RecipeShoppingLists_RecipeShoppingListId",
                table: "RecipeIngredientsShoppingLists",
                column: "RecipeShoppingListId",
                principalTable: "RecipeShoppingLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Categories_CategoryId",
                table: "Recipes",
                column: "CategoryId",
                principalTable: "Categories",
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
                name: "FK_RecipeIngredientsShoppingLists_RecipeShoppingLists_RecipeShoppingListId",
                table: "RecipeIngredientsShoppingLists");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Categories_CategoryId",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "RecipeShoppingLists");

            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "RecipeIngredientsShoppingLists");

            migrationBuilder.RenameColumn(
                name: "RecipeShoppingListId",
                table: "RecipeIngredientsShoppingLists",
                newName: "ShoppingListId");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeIngredientsShoppingLists_RecipeShoppingListId",
                table: "RecipeIngredientsShoppingLists",
                newName: "IX_RecipeIngredientsShoppingLists_ShoppingListId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "Recipes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredientsShoppingLists_ShoppingLists_ShoppingListId",
                table: "RecipeIngredientsShoppingLists",
                column: "ShoppingListId",
                principalTable: "ShoppingLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Categories_CategoryId",
                table: "Recipes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
