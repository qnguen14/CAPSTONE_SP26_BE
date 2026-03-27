using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("PayOS_Order_Item")]
public class PayOSOrderItem
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Order))]
    [Column("order_id")]
    public Guid OrderId { get; set; }
    public virtual PayOSOrder Order { get; set; }

    [Required]
    [Column("name")]
    [StringLength(256)]
    public string Name { get; set; }

    [Required]
    [Column("quantity")]
    public int Quantity { get; set; }

    [Required]
    [Column("price")]
    public int Price { get; set; }

    [Column("unit")]
    [StringLength(64)]
    public string? Unit { get; set; }

    [Column("tax_percentage")]
    public int? TaxPercentage { get; set; }
}
