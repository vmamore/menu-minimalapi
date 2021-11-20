using Menu;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MenuDB>(options => options.UseInMemoryDatabase("items"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Menu API",
        Description = "See the menu of yours favorite restaurants here!",
        Version = "v1" });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Menu API V1");
    c.RoutePrefix = string.Empty;
});

app.MapGet("/categories", async (MenuDB db) => await db.Categories.ToListAsync());
app.MapGet("/categories/{id}", async (MenuDB db, int id) => await db.Categories.FindAsync(id));
app.MapPost("/categories/{id}", async (MenuDB db, Category category) =>
{
    await db.Categories.AddAsync(category);
    await db.SaveChangesAsync();
    return Results.Created($"/categories/{category.Id}", category);
});
app.MapPut("/categories/{id}", async (MenuDB db, Category updatedCategory, int id) =>
{
    var category = await db.Categories.FindAsync(id);
    if (category is null) return Results.NotFound();
    category.Name = updatedCategory.Name;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/categories/{id}", async (MenuDB db, int id) =>
{
    var category = await db.Categories.FindAsync(id);
    if (category is null)
    {
        return Results.NotFound();
    }

    if (category.Foods.Any())
    {
        return Results.BadRequest("Category cannot be removed because it contain foods");
    }
    db.Categories.Remove(category);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/foods", async (MenuDB db) => await db.Foods.ToListAsync());
app.MapGet("/foods/{id}", async (MenuDB db, int id) => await db.Foods.FindAsync(id));
app.MapPost("/foods/{id}", async (MenuDB db, Food food) =>
{
    var category = await db.Categories.FindAsync(food.Category.Id);
    if (category is null)
    {
        return Results.NotFound("Category not found.");
    }
    await db.Foods.AddAsync(food);
    await db.SaveChangesAsync();
    return Results.Created($"/foods/{food.Id}", food);
});
app.MapPut("/foods/{id}", async (MenuDB db, Food updatedFood, int id) =>
{
    var category = await db.Categories.FindAsync(updatedFood.Category.Id);
    if (category is null)
    {
        return Results.NotFound("Category not found.");
    }

    var food = await db.Foods.FindAsync(id);
    if (food is null)
    {
        return Results.NotFound("Food not found.");
    }
    food.Name = updatedFood.Name;
    food.Description = updatedFood.Description;
    food.Category = category;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/foods/{id}", async (MenuDB db, int id) =>
{
    var food = await db.Foods.FindAsync(id);
    if (food is null)
    {
        return Results.NotFound();
    }
    db.Foods.Remove(food);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();
