using Microsoft.EntityFrameworkCore;
using Newsletter.Reporting.Api.Entities;

namespace Newsletter.Reporting.Api.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArticleEvent>().HasOne<Article>().WithMany();
    }

    public DbSet<Article> Articles { get; set; }

    public DbSet<ArticleEvent> ArticleEvents { get; set; }
}
