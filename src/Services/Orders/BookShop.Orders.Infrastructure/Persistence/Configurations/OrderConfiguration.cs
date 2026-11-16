
using BookShop.Orders.Domain.Aggregates;
using BookShop.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Orders.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration
    : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", schema: "orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .ValueGeneratedNever();

        builder.Property(order => order.CustomerId)
            .IsRequired();

        builder.HasIndex(order => order.CustomerId);

        builder.Property(order => order.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Ignore(order => order.TotalAmount);

        builder.Ignore(order => order.DomainEvents);

        builder.OwnsMany(order => order.OrderLines, line =>
        {
            line.ToTable("OrderLines", schema: "orders");

            line.WithOwner()
                .HasForeignKey("OrderId");

            line.Property<Guid>("OrderLineId")
                .ValueGeneratedOnAdd();

            line.HasKey("OrderId", "OrderLineId");

            line.Property(item => item.ProductId)
                .IsRequired();

            line.Property(item => item.Quantity)
                .IsRequired();

            line.Property(item => item.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            line.Ignore(item => item.Total);
        });

        builder.Navigation(order => order.OrderLines)
            .HasField("_orderLines")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
