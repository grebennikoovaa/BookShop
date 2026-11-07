namespace BookShop.Orders.Domain.Events;

public record OrderCancelled(
    Guid OrderId,
    DateTime OccurredAt) : IDomainEvent;