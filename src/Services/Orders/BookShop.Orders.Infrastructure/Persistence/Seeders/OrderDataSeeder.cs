
using BookShop.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookShop.Orders.Infrastructure.Persistence.Seeders;

internal sealed class OrderDataSeeder : IOrderDataSeeder
{
    private readonly OrderDbContext _dbContext;
    private readonly ILogger<OrderDataSeeder> _logger;

    public OrderDataSeeder(
        OrderDbContext dbContext,
        ILogger<OrderDataSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        // Prepare database.
        if (_dbContext.Database.IsRelational())
        {
            await _dbContext.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await _dbContext.Database.EnsureCreatedAsync(
                cancellationToken);
        }

        // Do not create duplicate starter orders.
        if (await _dbContext.Orders.AnyAsync(cancellationToken))
        {
            _logger.LogInformation(
                "Orders already exist. Seeding skipped.");

            return;
        }

        var now = DateTime.UtcNow;

        var customerId =
            Guid.Parse("11111111-1111-1111-1111-111111111111");

        var productId =
            Guid.Parse("22222222-2222-2222-2222-222222222222");

        // First order: payment will be received.
        var readyOrder = Order.PlaceOrder(
            customerId,
            new[]
            {
                (productId, 2, 25.00m)
            },
            now);

        var payment = Payment.InitiatePayment(
            readyOrder.Id,
            readyOrder.TotalAmount,
            now);

        payment.ConfirmPayment(
            "BOOKSHOP-SEED-PAYMENT-001",
            now.AddMinutes(1));

        // Second order: no payment.
        var unpaidOrder = Order.PlaceOrder(
            customerId,
            new[]
            {
                (productId, 1, 25.00m)
            },
            now);

        await _dbContext.Orders.AddRangeAsync(
            new[] { readyOrder, unpaidOrder },
            cancellationToken);

        await _dbContext.Payments.AddAsync(
            payment,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Seeded two BookShop orders. ReadyOrderId: {ReadyOrderId}; " +
            "UnpaidOrderId: {UnpaidOrderId}",
            readyOrder.Id,
            unpaidOrder.Id);
    }
}
