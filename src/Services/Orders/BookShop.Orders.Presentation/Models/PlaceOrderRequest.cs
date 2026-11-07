using System.ComponentModel.DataAnnotations;

namespace BookShop.Orders.Presentation.Models;

public class PlaceOrderRequest
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal UnitPrice { get; set; }
}