
using BookShop.Orders.Application.Contracts;
using BookShop.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Orders.Infrastructure.Persistence.Repositories;

internal sealed class SqlPaymentRepository : IPaymentRepository
{
    private readonly OrderDbContext _dbContext;

    public SqlPaymentRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Payment payment)
    {
        await _dbContext.Payments.AddAsync(payment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Payments
            .FirstOrDefaultAsync(payment => payment.Id == id);
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
    {
        return await _dbContext.Payments
            .FirstOrDefaultAsync(payment => payment.OrderId == orderId);
    }

    public async Task SaveAsync(Payment payment)
    {
        await _dbContext.SaveChangesAsync();
    }
}
