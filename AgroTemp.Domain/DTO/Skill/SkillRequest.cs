using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.DTO.Skill
{
    public class SkillRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(256)]
        public string Category { get; set; }

        public bool IsActive { get; set; }
    }
}
