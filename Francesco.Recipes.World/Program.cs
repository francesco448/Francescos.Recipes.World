using Francesco.Recipes.World.Data;
using Francesco.Recipes.World.Repositories.Category;
using Francesco.Recipes.World.Repositories.Favorit;
using Francesco.Recipes.World.Repositories.Ingredient;
using Francesco.Recipes.World.Repositories.Instruction;
using Francesco.Recipes.World.Repositories.MediaFile;
using Francesco.Recipes.World.Repositories.Recipe;
using Francesco.Recipes.World.Repositories.ShoppingList;
using Francesco.Recipes.World.Repositories.Unit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var configuration = builder.Configuration;

var connectionString = builder.Configuration.GetConnectionString("FrancescosRecipesWorldDbContextConnection")
                       ?? throw new InvalidOperationException("Connection string 'FrancescosRecipesWorldDbContextConnection' not found.");

services.AddDbContext<FrancescosRecipesWorldDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();

builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();

builder.Services.AddScoped<IUnitRepository, UnitRepository>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IShoppingListRepository, ShoppingListRepository>();

builder.Services.AddScoped<IMediaFileRepository, MediaFileRepository>();

builder.Services.AddScoped<IInstructionRepository, InstructionRepository>();

builder.Services.AddScoped<IFavoriteRepository, FavoritRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

Console.WriteLine("Standard Numeric Format Specifiers");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapDefaultControllerRoute();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Category}/{action=Index}/{id?}");

app.Run();
