using BookShop.Orders.Domain.Aggregates;
using BookShop.Orders.Domain.Events;
using BookShop.Orders.Domain.Exceptions;
using Xunit;

namespace BookShop.Orders.Domain.Tests.Aggregates;

public class PaymentTests
{
    private static readonly DateTime Now =
        new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Payment CreatePayment()
    {
        return Payment.InitiatePayment(
            Guid.NewGuid(),
            100m,
            Now);
    }

    [Fact]
    public void InitiatePayment_ShouldCreatePendingPayment()
    {
        var payment = CreatePayment();

        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Equal(100m, payment.Amount);
        Assert.Equal(Now.AddMinutes(15), payment.RetryDeadline);
    }

    [Fact]
    public void InitiatePayment_ShouldRaiseEvent()
    {
        var payment = CreatePayment();

        Assert.Contains(
            payment.DomainEvents,
            e => e is PaymentInitiated);
    }

    [Fact]
    public void RejectPayment_ShouldKeepPaymentPending()
    {
        var payment = CreatePayment();

        payment.RejectPayment(Now.AddMinutes(1));

        Assert.Equal(PaymentStatus.Pending, payment.Status);

        Assert.Contains(
            payment.DomainEvents,
            e => e is PaymentRejected);
    }

    [Fact]
    public void ConfirmPayment_ShouldReceivePayment_WhenWithinRetryWindow()
    {
        var payment = CreatePayment();

        payment.RejectPayment(Now.AddMinutes(1));

        payment.ConfirmPayment(
            "PROVIDER-123",
            Now.AddMinutes(2));

        Assert.Equal(PaymentStatus.Received, payment.Status);
        Assert.Equal("PROVIDER-123", payment.ProviderReference);
    }

    [Fact]
    public void ConfirmPayment_ShouldThrow_WhenRetryWindowExpired()
    {
        var payment = CreatePayment();

        var exception =
            Assert.Throws<PaymentRetryWindowExpiredException>(
                () => payment.ConfirmPayment(
                    "PROVIDER-123",
                    Now.AddMinutes(16)));

        Assert.Equal(payment.Id, exception.PaymentId);
        Assert.Equal(Now.AddMinutes(15), exception.RetryDeadline);
    }

    [Fact]
    public void ExpirePayment_ShouldChangeStatus_AfterDeadline()
    {
        var payment = CreatePayment();

        payment.ExpirePayment(Now.AddMinutes(16));

        Assert.Equal(PaymentStatus.Expired, payment.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void InitiatePayment_ShouldRejectInvalidAmount(decimal amount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Payment.InitiatePayment(
                Guid.NewGuid(),
                amount,
                Now));
    }
}