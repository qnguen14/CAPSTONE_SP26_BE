using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        public bool isPrimary { get; set; } = false;
    }
}
