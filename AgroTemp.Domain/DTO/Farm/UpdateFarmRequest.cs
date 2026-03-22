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

        /// <summary>
        /// 1 = Livestock (chăn nuôi), 2 = Crop (trồng trọt)
        /// </summary>
        public FarmType? FarmType { get; set; }

        /// <summary>
        /// Update number of animals (only for Livestock farms)
        /// </summary>
        public int? LivestockCount { get; set; }

        /// <summary>
        /// Update area in square meters (m²) (only for Crop farms)
        /// </summary>
        public decimal? AreaSize { get; set; }

        public bool? IsPrimary { get; set; }
    }
}
