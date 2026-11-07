namespace BookShop.Orders.Application.DTOs;

public record PaymentStatusResponse(
    Guid PaymentId,
    Guid OrderId,
    string Status
);