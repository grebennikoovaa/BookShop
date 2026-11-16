
namespace BookShop.Orders.Infrastructure.Persistence.Seeders;

public interface IOrderDataSeeder
{
    Task SeedAsync(
        CancellationToken cancellationToken = default);
}
