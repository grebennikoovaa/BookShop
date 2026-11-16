
using BookShop.Orders.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Orders.Infrastructure.Persistence.Configurations;

internal sealed class PaymentConfiguration
    : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", schema: "orders");

        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.Id)
            .ValueGeneratedNever();

        builder.Property(payment => payment.OrderId)
            .IsRequired();

        builder.HasIndex(payment => payment.OrderId)
            .IsUnique();

        builder.Property(payment => payment.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(payment => payment.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(payment => payment.RetryDeadline)
            .IsRequired();

        builder.Property(payment => payment.ProviderReference)
            .HasMaxLength(200);

        builder.Ignore(payment => payment.DomainEvents);
    }
}
