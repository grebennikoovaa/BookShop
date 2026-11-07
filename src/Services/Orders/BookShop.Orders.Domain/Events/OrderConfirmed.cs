namespace BookShop.Orders.Domain.Events;

public record OrderConfirmed(
    Guid OrderId,
    DateTime OccurredAt) : IDomainEvent;