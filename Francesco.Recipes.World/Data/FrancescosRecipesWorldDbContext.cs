namespace Francesco.Recipes.World.Data;

    using Francesco.Recipes.World.Models.BackendModels;
    using Francesco.Recipes.World.Models.BackendModels.Category;
    using Francesco.Recipes.World.Models.BackendModels.Favorit;
    using Francesco.Recipes.World.Models.BackendModels.Ingredient;
    using Francesco.Recipes.World.Models.BackendModels.IngredientShoppingList;
    using Francesco.Recipes.World.Models.BackendModels.Instruction;
    using Francesco.Recipes.World.Models.BackendModels.MediaFile;
    using Francesco.Recipes.World.Models.BackendModels.Recipe;
    using Francesco.Recipes.World.Models.BackendModels.RecipeIngredient;
    using Francesco.Recipes.World.Models.BackendModels.Shoppinglist;
    using Francesco.Recipes.World.Models.BackendModels.Unit;
    using Microsoft.EntityFrameworkCore;

    public class FrancescosRecipesWorldDbContext : DbContext
    {
        public FrancescosRecipesWorldDbContext(DbContextOptions<FrancescosRecipesWorldDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Ingredient> Ingredients => Set<Ingredient>();

        public DbSet<Instruction> Instructions => Set<Instruction>();

        public DbSet<Recipe> Recipes => Set<Recipe>();

        public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

        public DbSet<Unit> Units => Set<Unit>();

        public DbSet<Favorit> Favorits => Set<Favorit>();

        public DbSet<RecipeIngredientShoppingList> RecipeIngredientsShoppingLists => Set<RecipeIngredientShoppingList>();

        public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();

        public DbSet<MediaFile> MediaFiles => Set<MediaFile>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is ITimeStampedEntity timeStampedEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            timeStampedEntity.CreatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            timeStampedEntity.ModifiedAt = DateTime.UtcNow;
                            break;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            SeedData(builder);

            base.OnModelCreating(builder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
               .HasData(GetCategories());

            modelBuilder.Entity<Unit>()
                .HasData(GetUnits());
        }

        private static IEnumerable<Category> GetCategories()
        {
            return
            [
            new Category { Id = Guid.Parse("b248244f-f21c-4555-a14d-5dd49a2717cf"), Name = "Vorspeisen & Snacks" },
            new Category { Id = Guid.Parse("5186c5d6-5aff-4a6e-baf4-b61b72d889fe"), Name = "Erste Gänge" },
            new Category { Id = Guid.Parse("abbc6bb0-97ea-49f5-b31e-6507eb784fd1"), Name = "Hauptgerichte" },
            new Category { Id = Guid.Parse("90deec39-dcd0-422d-9018-ac8389e332e1"), Name = "Desserts & Süßspeisen" },
            new Category { Id = Guid.Parse("28e39168-701a-4084-81da-d96c987c462f"), Name = "Beilagen & Salate" },
            new Category { Id = Guid.Parse("20585b74-4805-4aff-a6df-aa6b7af04ff1"), Name = "Kuchen" },
            new Category { Id = Guid.Parse("adfb75ce-3ef3-428b-a5ca-b0c4c619d5ec"), Name = "Hefegebäck & Brot" },
            new Category { Id = Guid.Parse("d332f88d-d241-48c5-a2f2-bfd124eada7e"), Name = "Soßen & Saucen" },
            new Category { Id = Guid.Parse("23b1c740-e427-44f9-a6ea-d33d3f30f05a"), Name = "Marmeladen & Eingemachtes" },
            new Category { Id = Guid.Parse("0a91a200-dc76-4e00-b38c-b38cab5b69d7"), Name = "Getränke" },
        ];
        }

        private static IEnumerable<Unit> GetUnits()
        {
            return
            [
            new Unit { Id = Guid.Parse("62eb2c11-8768-46fb-9db0-c77f940bb4aa"), Name = "liter", Symbol = "l" },
            new Unit { Id = Guid.Parse("6c41f7e6-ca75-49cc-8541-cebd5a9c560b"), Name = "gramm", Symbol = "g" },
            new Unit { Id = Guid.Parse("7e3d1b86-ac48-45d6-814e-d3492a86db1d"), Name = "kilogramm", Symbol = "kg" },
            new Unit { Id = Guid.Parse("0c3648ec-4981-42c8-abf8-18bf1a2ff4c2"), Name = "stücke", Symbol = "stk" },
            new Unit { Id = Guid.Parse("d82f4abd-e4e4-4104-a9c0-1acdeaa701f5"), Name = "blatt", Symbol = "blatt" },
            new Unit { Id = Guid.Parse("df5cb4c3-4de6-4c6f-be8c-da41b2986408"), Name = "messerspitze", Symbol = "msp" },
            new Unit { Id = Guid.Parse("e45a3af2-2ed6-4ac4-b06f-b4175663a7be"), Name = "stange", Symbol = "stange" },
            new Unit { Id = Guid.Parse("7ea2f51d-7493-4f19-a663-1f309186d3ae"), Name = "bund", Symbol = "bund" },
            new Unit { Id = Guid.Parse("66556e0e-eb2b-4bc3-9a56-135dd508ed09"), Name = "zehe", Symbol = "zehe" },
            new Unit { Id = Guid.Parse("2e9894ac-14fa-43fd-ba07-35cdd8ebd461"), Name = "teelöffel", Symbol = "TL" },
            new Unit { Id = Guid.Parse("24a91b89-3389-4465-89e4-2f70d2ea6fd7"), Name = "esslöffel", Symbol = "EL" },
        ];
        }
    }
