namespace Teacing_api.Models;

using Microsoft.EntityFrameworkCore;


public class AppDbContext : DbContext
{
    public DbSet<Teacing_api.Models.Category> Category { get; set; } = default!;
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }


    public DbSet<Product> Products { get; set; }
}


