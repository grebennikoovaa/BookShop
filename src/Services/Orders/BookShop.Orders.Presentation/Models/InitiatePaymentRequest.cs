using System.ComponentModel.DataAnnotations;

namespace BookShop.Orders.Presentation.Models;

public class InitiatePaymentRequest
{
    [Required]
    public Guid OrderId { get; set; }

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal Amount { get; set; }
}