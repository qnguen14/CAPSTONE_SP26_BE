using AgroTemp.Domain.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroTemp.Domain.Entities;

[Table("Farm")]
public class Farm
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [ForeignKey(nameof(Farmer))]
    [Column("farmer_id")]
    public Guid FarmerId { get; set; }
    public virtual Farmer Farmer { get; set; }

    [Required]
    [Column("address")]
    public string Address { get; set; }

    [Required]
    [Column("latitude")]
    public decimal Latitude { get; set; }

    [Required]
    [Column("longitude")]
    public decimal Longitude { get; set; }

    [Required]
    [Column("farm_type")]
    public FarmType FarmType { get; set; }

    [Column("livestock_count")]
    public int? LivestockCount { get; set; }

    [Column("area_size")]
    public decimal? AreaSize { get; set; }

    [Required]
    [Column("location_name")]
    [StringLength(256)]
    public string LocationName { get; set; }

    [Column("image_url")]
    [StringLength(1024)]
    public List<string>? ImageUrl { get; set; }

    [Required]
    [Column("is_primary")]
    public bool IsPrimary { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
