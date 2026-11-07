using System.Collections.Concurrent;
using BookShop.Orders.Application.Contracts;
using BookShop.Orders.Domain.Aggregates;

namespace BookShop.Orders.Presentation.Infrastructure;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task AddAsync(Order order)
    {
        if (!_orders.TryAdd(order.Id, order))
            throw new InvalidOperationException("Order already exists.");

        return Task.CompletedTask;
    }

    public Task<Order?> GetByIdAsync(Guid id)
    {
        _orders.TryGetValue(id, out var order);

        return Task.FromResult(order);
    }

    public Task SaveAsync(Order order)
    {
        _orders[order.Id] = order;

        return Task.CompletedTask;
    }
}