using AgroTemp.Domain.Metadata;

namespace AgroTemp.Domain.DTO.Farm
{
    public class FarmDTO
    {
        public Guid Id { get; set; }
        public Guid FarmerProfileId { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string LocationName { get; set; }
        public FarmType FarmType { get; set; }
        public string FarmTypeName => FarmType.ToString();

        // Livestock farming: number of animals
        public int? LivestockCount { get; set; }

        // Crop farming: area in square meters (m²)
        public decimal? AreaSize { get; set; }

        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
