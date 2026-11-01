using System.Text.Json.Serialization;
using BookStore.Application;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("BookStore") ?? "Data Source=bookstore.db";
builder.Services.AddDbContext<BookStoreDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<IRepository<BookEntity, int>>(sp => sp.GetRequiredService<BookRepository>());
builder.Services.AddScoped<IReadOnlyRepository<BookEntity, int>>(sp => sp.GetRequiredService<BookRepository>());
builder.Services.AddScoped<IBookReadOnlyRepository>(sp => sp.GetRequiredService<BookRepository>());

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<ApplicationLayer>();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
    await context.Database.MigrateAsync();
    await BookStoreSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
