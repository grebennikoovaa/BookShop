namespace BookShop.Orders.Domain.Exceptions;

public sealed class OrderCannotBeCancelledException
    : DomainException
{
    public Guid OrderId { get; }

    public string CurrentStatus { get; }

    public OrderCannotBeCancelledException(
        Guid orderId,
        string currentStatus)
        : base(
            "ORDER_CANNOT_BE_CANCELLED",
            $"Order {orderId} cannot be cancelled from status {currentStatus}.")
    {
        OrderId = orderId;
        CurrentStatus = currentStatus;
    }
}