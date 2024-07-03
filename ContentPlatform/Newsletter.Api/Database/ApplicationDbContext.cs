using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Newsletter.Api.Entities;

namespace Newsletter.Api.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>().Property(p => p.Tags).HasConversion(
            to => JsonSerializer.Serialize(to, (JsonSerializerOptions?)null),
            from => JsonSerializer.Deserialize<List<string>>(from, (JsonSerializerOptions?)null)!);
    }

    public DbSet<Article> Articles { get; set; }
}
