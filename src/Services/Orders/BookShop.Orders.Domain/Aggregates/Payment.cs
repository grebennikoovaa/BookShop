using BookShop.Orders.Domain.Events;
using BookShop.Orders.Domain.Exceptions;
using BookShop.Orders.Domain.Shared;

namespace BookShop.Orders.Domain.Aggregates;

public sealed class Payment : AggregateRoot
{
    private const int RetryWindowMinutes = 15;

    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public decimal Amount { get; private set; }

    public PaymentStatus Status { get; private set; }

    public DateTime RetryDeadline { get; private set; }

    public string? ProviderReference { get; private set; }

    private Payment(Guid orderId, decimal amount, DateTime initiatedAt)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        RetryDeadline = initiatedAt.AddMinutes(RetryWindowMinutes);
    }

    public static Payment InitiatePayment(
        Guid orderId,
        decimal amount,
        DateTime initiatedAt)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID is required.");

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        var payment = new Payment(orderId, amount, initiatedAt);

        payment.AddDomainEvent(
            new PaymentInitiated(
                payment.Id,
                payment.OrderId,
                payment.Amount,
                initiatedAt));

        return payment;
    }

    public void RejectPayment(DateTime now)
    {
        EnsureRetryWindowIsOpen(now);

        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Payment is not pending.");

        AddDomainEvent(
            new PaymentRejected(Id, OrderId, now));
    }

    public void ConfirmPayment(
        string providerReference,
        DateTime now)
    {
        EnsureRetryWindowIsOpen(now);

        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Payment is not pending.");

        if (string.IsNullOrWhiteSpace(providerReference))
            throw new ArgumentException("Provider reference is required.");

        ProviderReference = providerReference;
        Status = PaymentStatus.Received;

        AddDomainEvent(
            new PaymentReceived(
                Id,
                OrderId,
                providerReference,
                now));
    }

    public void ExpirePayment(DateTime now)
    {
        if (Status != PaymentStatus.Pending || now < RetryDeadline)
            throw new InvalidOperationException("Payment cannot expire yet.");

        Status = PaymentStatus.Expired;
    }

    private void EnsureRetryWindowIsOpen(DateTime now)
    {
        if (now > RetryDeadline || Status == PaymentStatus.Expired)
        {
            throw new PaymentRetryWindowExpiredException(
                Id,
                RetryDeadline);
        }
    }
}