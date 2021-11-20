using Microsoft.EntityFrameworkCore;

namespace Menu;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Food>? Foods { get; set; }
}

public class Food
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
}

public class MenuDB : DbContext
{
    public MenuDB(DbContextOptions options) : base(options) { }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Food> Foods { get; set; }
}