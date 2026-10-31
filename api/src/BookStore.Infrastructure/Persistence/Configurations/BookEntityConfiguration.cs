using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations;

internal class BookEntityConfiguration: IEntityTypeConfiguration<BookEntity>
{
    public void Configure(EntityTypeBuilder<BookEntity> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Author).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Isbn).IsRequired().HasMaxLength(20);
        builder.Property(b => b.OpenLibraryKey).IsRequired().HasMaxLength(50);
        builder.HasIndex(b => b.OpenLibraryKey).IsUnique();        
        builder.HasIndex(b => b.Genre);
        builder.Property(b => b.Genre).HasConversion<string>().HasMaxLength(20);
        builder.Property(b => b.Price).HasPrecision(10, 2);
        builder.Property(b => b.CoverUrl).HasMaxLength(500);
        builder.Property(b => b.Description).HasMaxLength(5000);

    }
}
