using BookShop.Orders.Domain.Entities;
using BookShop.Orders.Domain.Events;
using BookShop.Orders.Domain.Exceptions;
using BookShop.Orders.Domain.Shared;

namespace BookShop.Orders.Domain.Aggregates;

public sealed class Order : AggregateRoot
{
    private readonly List<OrderLine> _orderLines = new();

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<OrderLine> OrderLines =>
        _orderLines.AsReadOnly();

    public decimal TotalAmount =>
        _orderLines.Sum(line => line.Total);

    private Order(Guid customerId, DateTime createdAt)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = createdAt;
    }

    public static Order PlaceOrder(
        Guid customerId,
        IEnumerable<(Guid ProductId, int Quantity, decimal UnitPrice)> items,
        DateTime createdAt)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID is required.");

        ArgumentNullException.ThrowIfNull(items);

        var order = new Order(customerId, createdAt);

        foreach (var item in items)
        {
            order._orderLines.Add(
                new OrderLine(
                    item.ProductId,
                    item.Quantity,
                    item.UnitPrice));
        }

        if (order._orderLines.Count == 0)
            throw new ArgumentException("Order must contain at least one item.");

        order.AddDomainEvent(
            new OrderPlaced(
                order.Id,
                order.CustomerId,
                order.TotalAmount,
                createdAt));

        return order;
    }

    public void ConfirmOrder(bool paymentReceived, DateTime now)
    {
        if (Status != OrderStatus.Pending || !paymentReceived)
        {
            throw new OrderCannotBeConfirmedException(
                Id,
                Status.ToString());
        }

        Status = OrderStatus.Confirmed;

        AddDomainEvent(new OrderConfirmed(Id, now));
    }

    public void CancelOrder(
        Guid? paymentId,
        DateTime now)
    {
        if (Status == OrderStatus.Cancelled)
        {
            throw new OrderCannotBeCancelledException(
                Id,
                Status.ToString());
        }

        Status = OrderStatus.Cancelled;

        AddDomainEvent(new OrderCancelled(Id, now));

        if (paymentId.HasValue)
        {
            AddDomainEvent(
                new RefundRequested(Id, paymentId.Value, now));
        }
    }
}