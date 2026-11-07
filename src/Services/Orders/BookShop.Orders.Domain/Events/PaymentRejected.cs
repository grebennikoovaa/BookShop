namespace BookShop.Orders.Domain.Events;

public record PaymentRejected(
    Guid PaymentId,
    Guid OrderId,
    DateTime OccurredAt) : IDomainEvent;