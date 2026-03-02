using Francesco.Recipes.World.Controller.Recipe;
using Francesco.Recipes.World.Data;
using Francesco.Recipes.World.Models;
using Francesco.Recipes.World.Models.BackendModels.Instruction;
using Francesco.Recipes.World.Models.BackendModels.MediaFile;
using Francesco.Recipes.World.Models.BackendModels.Recipe;
using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;
using Francesco.Recipes.World.Repositories.Category;
using Francesco.Recipes.World.Repositories.Favorit;
using Francesco.Recipes.World.Repositories.Ingredient;
using Francesco.Recipes.World.Repositories.Instruction;
using Francesco.Recipes.World.Repositories.MediaFile;
using Francesco.Recipes.World.Repositories.Recipe;
using Francesco.Recipes.World.Repositories.Unit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FrancescosRecipeWorld_Mock
{
    [TestClass]
    public sealed class FrancescoDamicoUnittest2
    {
        private Mock<IFavoriteRepository> _mockFavoriteRepository;
        private Mock<IRecipeRepository> _mockRecipeRepository;
        private RecipeController _recipeController;

        [TestInitialize]
        public void Setup()
        {
            _mockFavoriteRepository = new Mock<IFavoriteRepository>();
            _mockRecipeRepository = new Mock<IRecipeRepository>();

            var options = new DbContextOptionsBuilder<FrancescosRecipesWorldDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _recipeController = new RecipeController(
                _mockRecipeRepository.Object,
                Mock.Of<IUnitRepository>(),
                Mock.Of<ICategoryRepository>(),
                Mock.Of<IIngredientRepository>(),
                Mock.Of<IMediaFileRepository>(),
                Mock.Of<IInstructionRepository>(),
                _mockFavoriteRepository.Object,
                new FrancescosRecipesWorldDbContext(options)
            );
        }

        /// <summary>
        /// Test 1: AddFavorite(Guid recipeId) -> PartialViewResult mit FavoriteButtonViewModel
        /// Parameter: recipeId (Guid)
        /// Rückgabewert: PartialViewResult
        /// </summary>
        [TestMethod]
        public async Task FrancescoDamico_UnitTest1()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            _mockFavoriteRepository
                .Setup(x => x.AddFavoriteAsync(recipeId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _recipeController.AddFavorite(recipeId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var partialViewResult = result as PartialViewResult;
            Assert.AreEqual("_FavoriteButton", partialViewResult.ViewName);
            Assert.IsInstanceOfType(partialViewResult.Model, typeof(FavoritButtonViewModel));

            var model = partialViewResult.Model as FavoritButtonViewModel;
            Assert.AreEqual(recipeId, model.Id);
            Assert.IsTrue(model.IsFavorite);
        }

        /// <summary>
        /// Test 2: RemoveFavorite(Guid recipeId) -> PartialViewResult mit FavoriteButtonViewModel
        /// Parameter: recipeId (Guid)
        /// Rückgabewert: PartialViewResult
        /// </summary>
        [TestMethod]
        public async Task FrancescoDamico_UnitTest2()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            _mockFavoriteRepository
                .Setup(x => x.RemoveFavoriteAsync(recipeId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _recipeController.RemoveFavorite(recipeId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var partialViewResult = result as PartialViewResult;
            Assert.AreEqual("_FavoriteButton", partialViewResult.ViewName);
            Assert.IsInstanceOfType(partialViewResult.Model, typeof(FavoritButtonViewModel));

            var model = partialViewResult.Model as FavoritButtonViewModel;
            Assert.AreEqual(recipeId, model.Id);
            Assert.IsFalse(model.IsFavorite);
        }

        /// <summary>
        /// Test 3: Details(Guid recipeId) -> ViewResult mit Recipe Modell
        /// Parameter: recipeId (Guid)
        /// Rückgabewert: ViewResult mit Recipe-Objekt
        /// </summary>
        [TestMethod]
        public async Task FrancescoDamico_UnitTest3()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var expectedRecipe = new Recipe
            {
                Id = recipeId,
                Name = "Spaghetti Carbonara",
                Description = "Klassisches italienisches Pasta-Gericht",
                Difficulty = Difficulty.Easy,
                Servings = 4,
                PreparationTime = new TimeSpan(0, 10, 0),
                CookingTime = new TimeSpan(0, 20, 0),
                IsFavorite = false,
                CreatedAt = DateTime.UtcNow,
                RecipeIngredients = new List<RecipeIngredient>(),
                Instructions = new List<Instruction>(),
                MediaFiles = new List<MediaFile>()
            };

            _mockRecipeRepository
                .Setup(x => x.GetRecipeByIdAsync(recipeId))
                .ReturnsAsync(expectedRecipe);

            // Act
            var result = await _recipeController.Details(recipeId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(Recipe));

            var model = viewResult.Model as Recipe;
            Assert.AreEqual(recipeId, model.Id);
            Assert.AreEqual("Spaghetti Carbonara", model.Name);
            Assert.AreEqual(Difficulty.Easy, model.Difficulty);
        }
    }
}
