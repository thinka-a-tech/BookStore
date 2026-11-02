using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence;

public static class BookStoreSeeder
{
    public static async Task SeedAsync(BookStoreDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Books.AnyAsync(cancellationToken))
        {
            return;
        }

        context.Books.AddRange(
            Book("Harry Potter and the Sorcerer's Stone", "J.K. Rowling", "9780590353403", "/works/OL82563W", Genre.Fantasy, 14.50m, 10, 1998),
            Book("Harry Potter and the Chamber of Secrets", "J.K. Rowling", "9780439064866", "/works/OL82537W", Genre.Fantasy, 13.50m, 8, 1999),
            Book("Harry Potter and the Prisoner of Azkaban", "J.K. Rowling", "9780439136358", "/works/OL82536W", Genre.Fantasy, 12.50m, 3, 1999),
            Book("Harry Potter and the Goblet of Fire", "J.K. Rowling", "9780439139595", "/works/OL82560W", Genre.Fantasy, 11.50m, 10, 2000),
            Book("Harry Potter and the Order of the Phoenix", "J.K. Rowling", "9780439139595", "/works/OL82548W", Genre.Fantasy, 9.80m, 15, 2003),
            Book("Harry Potter and the Half-Blood Prince", "J.K. Rowling", "9780439784542", "/works/OL82565W", Genre.Fantasy, 10.50m, 18, 2005),
            Book("Harry Potter and the Deathly Hallows", "J.K. Rowling", "9780545010221", "/works/OL82586W", Genre.Fantasy, 11.20m, 3, 2007),
            Book("Prey", "Michael Crichton", "9780066214122", "/works/OL46883W", Genre.Fiction, 7.99m, 20, 2002),
            Book("And Then There Were None", "Agatha Christie", "9780062325549", "/works/OL471565W", Genre.Mystery, 11.25m, 8, 2021),
            Book("Elon Musk", "Walter Isaacson", "9781982181284", "/works/OL35720357W", Genre.Biography, 16.75m, 12, 2023),
            Book("Sapiens: A Brief History of Humankind", "Yuval Noah Harari", "9780062316110", "/works/OL17075811W", Genre.History, 11.49m, 7, 2018),
            Book("Homo Deus: A Brief History of Tomorrow", "Yuval Noah Harari", "9780062464316", "/works/OL17641518W", Genre.History, 11.49m, 7, 2018),
            Book("The Midnight Library", "Matt Haig", "9780525559474", "/works/OL20965973W", Genre.SciFi, 18.49m, 45, 2020),
            Book("How to Stop Time", "Matt Haig", "9780525522874", "/works/OL19730220W", Genre.SciFi, 16.49m, 30, 2018));

        await context.SaveChangesAsync(cancellationToken);
    }

    private static BookEntity Book(string title, string author, string isbn, string key, Genre genre, decimal price, int stock, int year)
    {
        return new BookEntity(title, author, isbn, key, genre, price, stock, year, $"https://covers.openlibrary.org/b/isbn/{isbn}-M.jpg");
    }
}
