namespace BookShop.Orders.Domain.Exceptions;

public sealed class OrderCannotBeConfirmedException
    : DomainException
{
    public Guid OrderId { get; }

    public string CurrentStatus { get; }

    public OrderCannotBeConfirmedException(
        Guid orderId,
        string currentStatus)
        : base(
            "ORDER_CANNOT_BE_CONFIRMED",
            $"Order {orderId} cannot be confirmed from status {currentStatus}.")
    {
        OrderId = orderId;
        CurrentStatus = currentStatus;
    }
}