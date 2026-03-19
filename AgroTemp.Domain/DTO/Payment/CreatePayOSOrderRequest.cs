using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Payment;

public class CreatePayOSOrderRequest
{
    [Range(1, int.MaxValue)]
    public int TotalAmount { get; set; }

    public string? Description { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }

    public string? BuyerName { get; set; }
    public string? BuyerCompanyName { get; set; }
    public string? BuyerEmail { get; set; }
    public string? BuyerPhone { get; set; }
    public string? BuyerAddress { get; set; }

    public DateTimeOffset? ExpiredAt { get; set; }
    public bool? BuyerNotGetInvoice { get; set; }
    public int? TaxPercentage { get; set; }

    [MinLength(1)]
    public List<CreatePayOSOrderItemRequest> Items { get; set; } = new();
}
