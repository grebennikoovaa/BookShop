using System.ComponentModel.DataAnnotations;

namespace BookShop.Orders.Presentation.Models;

public class ConfirmPaymentRequest
{
    [Required]
    public string ProviderReference { get; set; } = string.Empty;
}