
using BookShop.Orders.Application.Contracts;
using BookShop.Orders.Infrastructure.Persistence;
using BookShop.Orders.Infrastructure.Persistence.Repositories;
using BookShop.Orders.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookShop.Orders.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
            var useInMemory = bool.TryParse(configuration["Persistence:UseInMemoryDatabase"],
        out var result) && result;

        services.AddDbContext<OrderDbContext>(options =>
        {
            if (useInMemory)
            {
                var databaseName =
                    configuration["Persistence:InMemoryDatabaseName"]
                    ?? "BookShopOrdersDb";

                options.UseInMemoryDatabase(databaseName);
            }
            else
            {
                var connectionString =
                    configuration.GetConnectionString("OrdersDb")
                    ?? throw new InvalidOperationException(
                        "Missing connection string 'OrdersDb'.");

                options.UseSqlServer(connectionString);
            }
        });

        services.AddScoped<IOrderRepository, SqlOrderRepository>();
        services.AddScoped<IPaymentRepository, SqlPaymentRepository>();
        services.AddScoped<IOrderDataSeeder, OrderDataSeeder>();

        return services;
    }
}
