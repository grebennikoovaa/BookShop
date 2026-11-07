using BookShop.Orders.Application.Contracts;
using BookShop.Orders.Application.Services;
using BookShop.Orders.Domain.Aggregates;
using Moq;
using Xunit;

namespace BookShop.Orders.Application.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly Mock<IPaymentRepository> _paymentRepository;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _paymentRepository = new Mock<IPaymentRepository>();

        _service = new OrderService(
            _orderRepository.Object,
            _paymentRepository.Object);
    }

    [Fact]
    public async Task PlaceOrderAsync_ShouldSaveOrder()
    {
        _orderRepository
            .Setup(x => x.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        var result = await _service.PlaceOrderAsync(
            Guid.NewGuid(),
            new[]
            {
                (Guid.NewGuid(), 2, 25m)
            });

        Assert.Equal(50m, result.TotalAmount);
        Assert.Equal("Pending", result.Status);

        _orderRepository.Verify(
            x => x.AddAsync(It.IsAny<Order>()),
            Times.Once);
    }

    [Fact]
    public async Task CancelOrderAsync_ShouldCancelAndSaveOrder()
    {
        var order = CreateOrder();

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id))
            .ReturnsAsync(order);

        _paymentRepository
            .Setup(x => x.GetByOrderIdAsync(order.Id))
            .ReturnsAsync((Payment?)null);

        _orderRepository
            .Setup(x => x.SaveAsync(order))
            .Returns(Task.CompletedTask);

        await _service.CancelOrderAsync(order.Id);

        Assert.Equal(OrderStatus.Cancelled, order.Status);

        _orderRepository.Verify(
            x => x.SaveAsync(order),
            Times.Once);
    }

    [Fact]
    public async Task InitiatePaymentAsync_ShouldSavePayment()
    {
        var order = CreateOrder();

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id))
            .ReturnsAsync(order);

        _paymentRepository
            .Setup(x => x.GetByOrderIdAsync(order.Id))
            .ReturnsAsync((Payment?)null);

        _paymentRepository
            .Setup(x => x.AddAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);

        var result = await _service.InitiatePaymentAsync(
            order.Id,
            order.TotalAmount);

        Assert.Equal("Pending", result.Status);

        _paymentRepository.Verify(
            x => x.AddAsync(It.IsAny<Payment>()),
            Times.Once);
    }

    [Fact]
    public async Task ConfirmPaymentAsync_ShouldSaveReceivedPayment()
    {
        var payment = Payment.InitiatePayment(
            Guid.NewGuid(),
            50m,
            DateTime.UtcNow);

        _paymentRepository
            .Setup(x => x.GetByIdAsync(payment.Id))
            .ReturnsAsync(payment);

        _paymentRepository
            .Setup(x => x.SaveAsync(payment))
            .Returns(Task.CompletedTask);

        await _service.ConfirmPaymentAsync(
            payment.Id,
            "PROVIDER-123");

        Assert.Equal(PaymentStatus.Received, payment.Status);

        _paymentRepository.Verify(
            x => x.SaveAsync(payment),
            Times.Once);
    }

    [Fact]
    public async Task GetOrderSummaryAsync_ShouldReturnSummary()
    {
        var order = CreateOrder();

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id))
            .ReturnsAsync(order);

        var result =
            await _service.GetOrderSummaryAsync(order.Id);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(50m, result.TotalAmount);
    }

    [Fact]
    public async Task RejectPaymentAsync_ShouldSaveRejectedAttempt()
    {
        var payment = Payment.InitiatePayment(
            Guid.NewGuid(),
            50m,
            DateTime.UtcNow);

        _paymentRepository
            .Setup(x => x.GetByIdAsync(payment.Id))
            .ReturnsAsync(payment);

        _paymentRepository
            .Setup(x => x.SaveAsync(payment))
            .Returns(Task.CompletedTask);

        await _service.RejectPaymentAsync(payment.Id);

        Assert.Equal(PaymentStatus.Pending, payment.Status);

        Assert.Contains(
            payment.DomainEvents,
            e => e is BookShop.Orders.Domain.Events.PaymentRejected);

        _paymentRepository.Verify(
            x => x.SaveAsync(payment),
            Times.Once);
    }

    private static Order CreateOrder()
    {
        return Order.PlaceOrder(
            Guid.NewGuid(),
            new[]
            {
                (Guid.NewGuid(), 2, 25m)
            },
            DateTime.UtcNow);
    }
}