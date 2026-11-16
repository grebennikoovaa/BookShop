
using BookShop.Orders.Application.Contracts;
using BookShop.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Orders.Infrastructure.Persistence.Repositories;

internal sealed class SqlOrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;

    public SqlOrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Orders
            .Include(order => order.OrderLines)
            .FirstOrDefaultAsync(order => order.Id == id);
    }

    public async Task SaveAsync(Order order)
    {
        await _dbContext.SaveChangesAsync();
    }
}
