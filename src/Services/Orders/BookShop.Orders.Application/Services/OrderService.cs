using BookShop.Orders.Application.Contracts;
using BookShop.Orders.Application.DTOs;
using BookShop.Orders.Domain.Aggregates;

namespace BookShop.Orders.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<OrderSummary> PlaceOrderAsync(
        Guid customerId,
        IEnumerable<(Guid ProductId, int Quantity, decimal UnitPrice)> items)
    {
        var order = Order.PlaceOrder(
            customerId,
            items,
            DateTime.UtcNow);

        await _orderRepository.AddAsync(order);

        return MapOrder(order);
    }

    public async Task CancelOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new KeyNotFoundException("Order not found.");

        var payment =
            await _paymentRepository.GetByOrderIdAsync(orderId);

        Guid? paymentId =
            payment?.Status == PaymentStatus.Received
                ? payment.Id
                : null;

        order.CancelOrder(paymentId, DateTime.UtcNow);

        await _orderRepository.SaveAsync(order);
    }

    public async Task<OrderSummary> ConfirmOrderAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new KeyNotFoundException("Order not found.");

        var payment =
            await _paymentRepository.GetByOrderIdAsync(orderId);

        bool paymentReceived =
            payment?.Status == PaymentStatus.Received;

        order.ConfirmOrder(paymentReceived, DateTime.UtcNow);

        await _orderRepository.SaveAsync(order);

        return MapOrder(order);
    }

    public async Task<PaymentStatusResponse> InitiatePaymentAsync(
        Guid orderId,
        decimal amount)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Payment can only be initiated for a pending order.");
        }

        if (amount != order.TotalAmount)
        {
            throw new InvalidOperationException(
                "Payment amount must match the order total.");
        }

        var existingPayment =
            await _paymentRepository.GetByOrderIdAsync(orderId);

        if (existingPayment is not null)
        {
            throw new InvalidOperationException(
                "Payment has already been initiated.");
        }

        var payment = Payment.InitiatePayment(
            orderId,
            amount,
            DateTime.UtcNow);

        await _paymentRepository.AddAsync(payment);

        return new PaymentStatusResponse(
            payment.Id,
            payment.OrderId,
            payment.Status.ToString());
    }

    public async Task ConfirmPaymentAsync(
        Guid paymentId,
        string providerReference)
    {
        var payment =
            await _paymentRepository.GetByIdAsync(paymentId)
            ?? throw new KeyNotFoundException("Payment not found.");

        payment.ConfirmPayment(
            providerReference,
            DateTime.UtcNow);

        await _paymentRepository.SaveAsync(payment);
    }

    public async Task RejectPaymentAsync(Guid paymentId)
    {
        var payment =
            await _paymentRepository.GetByIdAsync(paymentId)
            ?? throw new KeyNotFoundException("Payment not found.");

        payment.RejectPayment(DateTime.UtcNow);

        await _paymentRepository.SaveAsync(payment);
    }

    public async Task<OrderSummary?> GetOrderSummaryAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        return order is null ? null : MapOrder(order);
    }

    public async Task<PaymentDetails?> GetPaymentDetailsAsync(Guid paymentId)
    {
        var payment =
            await _paymentRepository.GetByIdAsync(paymentId);

        if (payment is null)
            return null;

        return new PaymentDetails(
            payment.Id,
            payment.OrderId,
            payment.Amount,
            payment.Status.ToString(),
            payment.ProviderReference);
    }

    private static OrderSummary MapOrder(Order order)
    {
        return new OrderSummary(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.TotalAmount,
            order.OrderLines.Count);
    }
}