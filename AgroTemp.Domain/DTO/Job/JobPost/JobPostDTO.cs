using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgroTemp.Domain.DTO.Farm;
using AgroTemp.Domain.DTO.FarmerProfile;

namespace AgroTemp.Domain.DTO.Job.JobPost
{
    public class JobPostDTO
    {
        public Guid Id { get; set; }
        public Guid FarmerProfileId { get; set; }
        public FarmerProfileSummaryDTO FarmerProfile { get; set; }
        public string ContactName { get; set; }
        public List<JobSkillRequirementSummaryDTO> JobSkillRequirements { get; set; } = new();
        public Guid FarmId { get; set; }
        public Guid JobCategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public List<DateOnly> SelectedDays { get; set; } = new List<DateOnly>();
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int WorkersNeeded { get; set; }
        public int WorkersAccepted { get; set; }
        public int JobTypeId { get; set; }
        public decimal WageAmount { get; set; }
        public List<string>? Requirements { get; set; } = new List<string>();
        public List<string>? Privileges { get; set; } = new List<string>();
        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsUrgent { get; set; }
        public int StatusId { get; set; }
    }
}
