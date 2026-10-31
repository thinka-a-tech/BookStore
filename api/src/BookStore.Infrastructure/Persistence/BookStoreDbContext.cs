using BookStore.Domain.Entities;
using BookStore.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence;

public class BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : DbContext(options)
{
    public DbSet<BookEntity> Books => Set<BookEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BookEntityConfiguration());
    }
}
