using System.Collections.Concurrent;
using BookShop.Orders.Application.Contracts;
using BookShop.Orders.Domain.Aggregates;

namespace BookShop.Orders.Presentation.Infrastructure;

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<Guid, Payment> _payments = new();

    public Task AddAsync(Payment payment)
    {
        if (!_payments.TryAdd(payment.Id, payment))
            throw new InvalidOperationException("Payment already exists.");

        return Task.CompletedTask;
    }

    public Task<Payment?> GetByIdAsync(Guid id)
    {
        _payments.TryGetValue(id, out var payment);

        return Task.FromResult(payment);
    }

    public Task<Payment?> GetByOrderIdAsync(Guid orderId)
    {
        var payment = _payments.Values
            .FirstOrDefault(p => p.OrderId == orderId);

        return Task.FromResult(payment);
    }

    public Task SaveAsync(Payment payment)
    {
        _payments[payment.Id] = payment;

        return Task.CompletedTask;
    }
}