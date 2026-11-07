using BookShop.Orders.Domain.Aggregates;
using BookShop.Orders.Domain.Events;
using BookShop.Orders.Domain.Exceptions;
using Xunit;

namespace BookShop.Orders.Domain.Tests.Aggregates;

public class OrderTests
{
    private static readonly DateTime Now =
        new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Order CreateOrder()
    {
        return Order.PlaceOrder(
            Guid.NewGuid(),
            new[]
            {
                (Guid.NewGuid(), 2, 25m)
            },
            Now);
    }

    [Fact]
    public void PlaceOrder_ShouldCreateOrder_WhenDataIsValid()
    {
        var order = CreateOrder();

        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Single(order.OrderLines);
        Assert.Equal(50m, order.TotalAmount);
    }

    [Fact]
    public void PlaceOrder_ShouldRaiseOrderPlacedEvent()
    {
        var order = CreateOrder();

        Assert.Contains(
            order.DomainEvents,
            e => e is OrderPlaced);
    }

    [Fact]
    public void ConfirmOrder_ShouldConfirm_WhenPaymentReceived()
    {
        var order = CreateOrder();

        order.ConfirmOrder(true, Now);

        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void ConfirmOrder_ShouldThrow_WhenPaymentNotReceived()
    {
        var order = CreateOrder();

        var exception =
            Assert.Throws<OrderCannotBeConfirmedException>(
                () => order.ConfirmOrder(false, Now));

        Assert.Equal(order.Id, exception.OrderId);
        Assert.Equal("Pending", exception.CurrentStatus);
        Assert.Equal(
            "ORDER_CANNOT_BE_CONFIRMED",
            exception.Code);
    }

    [Fact]
    public void CancelOrder_ShouldCancel_WhenOrderIsPending()
    {
        var order = CreateOrder();

        order.CancelOrder(null, Now);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void CancelOrder_ShouldRaiseRefundRequested_WhenPaymentExists()
    {
        var order = CreateOrder();
        var paymentId = Guid.NewGuid();

        order.CancelOrder(paymentId, Now);

        Assert.Contains(
            order.DomainEvents,
            e => e is RefundRequested);
    }

    [Fact]
    public void CancelOrder_ShouldThrow_WhenAlreadyCancelled()
    {
        var order = CreateOrder();

        order.CancelOrder(null, Now);

        var exception =
            Assert.Throws<OrderCannotBeCancelledException>(
                () => order.CancelOrder(null, Now));

        Assert.Equal(order.Id, exception.OrderId);
        Assert.Equal("Cancelled", exception.CurrentStatus);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void PlaceOrder_ShouldRejectInvalidQuantity(int quantity)
    {
        var items = new[]
        {
            (Guid.NewGuid(), quantity, 25m)
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => Order.PlaceOrder(Guid.NewGuid(), items, Now));
    }
}