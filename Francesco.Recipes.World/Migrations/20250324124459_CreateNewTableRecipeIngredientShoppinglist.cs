using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Francesco.Recipes.World.Migrations
{
    /// <inheritdoc />
    public partial class CreateNewTableRecipeIngredientShoppinglist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropTable(
                name: "IngredientsShoppingLists");

            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "Instructions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "RecipeIngredientsShoppingLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShoppingListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipeIngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredientsShoppingLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeIngredientsShoppingLists_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecipeIngredientsShoppingLists_RecipeIngredients_RecipeIngredientId",
                        column: x => x.RecipeIngredientId,
                        principalTable: "RecipeIngredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeIngredientsShoppingLists_ShoppingLists_ShoppingListId",
                        column: x => x.ShoppingListId,
                        principalTable: "ShoppingLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredientsShoppingLists_IngredientId",
                table: "RecipeIngredientsShoppingLists",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredientsShoppingLists_RecipeIngredientId",
                table: "RecipeIngredientsShoppingLists",
                column: "RecipeIngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredientsShoppingLists_ShoppingListId",
                table: "RecipeIngredientsShoppingLists",
                column: "ShoppingListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropTable(
                name: "RecipeIngredientsShoppingLists");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Instructions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "IngredientsShoppingLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShoppinglistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientsShoppingLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientsShoppingLists_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientsShoppingLists_ShoppingLists_ShoppinglistId",
                        column: x => x.ShoppinglistId,
                        principalTable: "ShoppingLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientsShoppingLists_IngredientId",
                table: "IngredientsShoppingLists",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientsShoppingLists_ShoppinglistId",
                table: "IngredientsShoppingLists",
                column: "ShoppinglistId");
        }
    }
}
