namespace BookShop.Orders.Domain.Events;

public record OrderPlaced(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    DateTime OccurredAt) : IDomainEvent;