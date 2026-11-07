namespace BookShop.Orders.Domain.Entities;

public sealed class OrderLine
{
    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Total => Quantity * UnitPrice;

    internal OrderLine(
        Guid productId,
        int quantity,
        decimal unitPrice)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID is required.");

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice));

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}