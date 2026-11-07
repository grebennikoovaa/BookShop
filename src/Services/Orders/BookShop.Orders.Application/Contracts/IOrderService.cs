using BookShop.Orders.Application.DTOs;

namespace BookShop.Orders.Application.Contracts;

public interface IOrderService
{
    Task<OrderSummary> PlaceOrderAsync(
        Guid customerId,
        IEnumerable<(Guid ProductId, int Quantity, decimal UnitPrice)> items);

    Task CancelOrderAsync(Guid orderId);

    Task<OrderSummary> ConfirmOrderAsync(Guid orderId);

    Task<PaymentStatusResponse> InitiatePaymentAsync(
        Guid orderId,
        decimal amount);

    Task ConfirmPaymentAsync(
        Guid paymentId,
        string providerReference);

    Task RejectPaymentAsync(Guid paymentId);

    Task<OrderSummary?> GetOrderSummaryAsync(Guid orderId);

    Task<PaymentDetails?> GetPaymentDetailsAsync(Guid paymentId);
}