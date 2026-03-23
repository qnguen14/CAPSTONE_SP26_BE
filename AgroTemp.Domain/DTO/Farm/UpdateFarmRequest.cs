using AgroTemp.Domain.Metadata;
using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.Farm
{
    public class UpdateFarmRequest
    {
        public string? Address { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        [StringLength(256)]
        public string? LocationName { get; set; }

        public FarmType? FarmType { get; set; }

        public int? LivestockCount { get; set; }

        public decimal? AreaSize { get; set; }

        public bool? IsPrimary { get; set; }
    }
}
