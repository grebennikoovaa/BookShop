namespace BookShop.Orders.Domain.Exceptions;

public sealed class PaymentRetryWindowExpiredException
    : DomainException
{
    public Guid PaymentId { get; }

    public DateTime RetryDeadline { get; }

    public PaymentRetryWindowExpiredException(
        Guid paymentId,
        DateTime retryDeadline)
        : base(
            "PAYMENT_RETRY_WINDOW_EXPIRED",
            $"Payment retry window expired at {retryDeadline:O}.")
    {
        PaymentId = paymentId;
        RetryDeadline = retryDeadline;
    }
}