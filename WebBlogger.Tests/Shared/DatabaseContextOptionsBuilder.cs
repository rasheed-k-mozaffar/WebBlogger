using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WebBlogger.Tests.Shared;

public static class DatabaseContextOptionsBuilder
{
    public static DbContextOptions<AppDbContext> CreateNewContextOptions(string databaseName)
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseInMemoryDatabase(databaseName)
            .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }
}