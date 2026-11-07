namespace BookShop.Orders.Domain.Events;

public record RefundRequested(
    Guid OrderId,
    Guid PaymentId,
    DateTime OccurredAt) : IDomainEvent;