using BookStore.Application.Features.Catalog;
using BookStore.Infrastructure.ExternalServices.OpenLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenLibraryCatalog(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenLibraryOptions>(configuration.GetSection(OpenLibraryOptions.SectionName));

        services.AddHttpClient<IBookCatalog, OpenLibraryClient>((sp, client) =>
        {
            var options = configuration.GetSection(OpenLibraryOptions.SectionName).Get<OpenLibraryOptions>() ?? new OpenLibraryOptions();
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
        });

        return services;
    }
}
