using AgroTemp.Domain.Metadata;
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

        /// <summary>
        /// 1 = Livestock (chăn nuôi), 2 = Crop (trồng trọt)
        /// </summary>
        [Required]
        public FarmType FarmType { get; set; }

        /// <summary>
        /// Required when FarmType = Livestock. Number of animals.
        /// </summary>
        public int? LivestockCount { get; set; }

        /// <summary>
        /// Required when FarmType = Crop. Area in square meters (m²).
        /// </summary>
        public decimal? AreaSize { get; set; }

        public bool isPrimary { get; set; } = false;
    }
}
