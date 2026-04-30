using System;
using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Farm
{
    public class CreateFarmRequest
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        [Required]
        [StringLength(256)]
        public string LocationName { get; set; }

        public List<string>? ImageUrl { get; set; }

        [Required]
        public Guid FarmTypeId { get; set; }

        public int? LivestockCount { get; set; }

        public decimal? AreaSize { get; set; }

        public bool isPrimary { get; set; } = false;
    }
}
