using BookShop.Orders.Domain.Aggregates;

namespace BookShop.Orders.Application.Contracts;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);

    Task<Payment?> GetByIdAsync(Guid id);

    Task<Payment?> GetByOrderIdAsync(Guid orderId);

    Task SaveAsync(Payment payment);
}