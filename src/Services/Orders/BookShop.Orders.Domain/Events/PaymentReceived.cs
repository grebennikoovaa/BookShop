namespace BookShop.Orders.Domain.Events;

public record PaymentReceived(
    Guid PaymentId,
    Guid OrderId,
    string ProviderReference,
    DateTime OccurredAt) : IDomainEvent;