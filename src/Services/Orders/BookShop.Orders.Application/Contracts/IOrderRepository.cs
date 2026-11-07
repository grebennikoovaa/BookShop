using BookShop.Orders.Domain.Aggregates;

namespace BookShop.Orders.Application.Contracts;

public interface IOrderRepository
{
    Task AddAsync(Order order);

    Task<Order?> GetByIdAsync(Guid id);

    Task SaveAsync(Order order);
}