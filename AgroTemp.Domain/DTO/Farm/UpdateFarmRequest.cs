using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.DTO.Farm
{
    public class UpdateFarmRequest
    {
        public string? Address { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        [StringLength(256)]
        public string? LocationName { get; set; }

        public bool? IsPrimary { get; set; }
    }
}
