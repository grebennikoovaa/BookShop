namespace BookShop.Orders.Application.DTOs;

public record PaymentDetails(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string Status,
    string? ProviderReference
);