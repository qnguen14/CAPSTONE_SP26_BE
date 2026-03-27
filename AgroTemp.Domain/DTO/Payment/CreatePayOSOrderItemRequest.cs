using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Payment;

public class CreatePayOSOrderItemRequest
{
    [Required]
    public string Name { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, int.MaxValue)]
    public int Price { get; set; }

    public string? Unit { get; set; }

    public int? TaxPercentage { get; set; }
}
