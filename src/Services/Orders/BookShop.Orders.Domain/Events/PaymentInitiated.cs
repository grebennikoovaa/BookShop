namespace BookShop.Orders.Domain.Events;

public record PaymentInitiated(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    DateTime OccurredAt) : IDomainEvent;