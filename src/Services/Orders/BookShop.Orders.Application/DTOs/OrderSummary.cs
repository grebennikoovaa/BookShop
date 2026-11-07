namespace BookShop.Orders.Application.DTOs;

public record OrderSummary(
    Guid Id,
    Guid CustomerId,
    string Status,
    decimal TotalAmount,
    int ItemsCount
);