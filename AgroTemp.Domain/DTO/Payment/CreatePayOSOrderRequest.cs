using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Payment;

public class CreatePayOSOrderRequest
{
    [Range(1, int.MaxValue)]
    public int TotalAmount { get; set; }

    public string? Description { get; set; }
}
