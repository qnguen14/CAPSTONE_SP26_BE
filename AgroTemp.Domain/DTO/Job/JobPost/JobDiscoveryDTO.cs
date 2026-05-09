using System;
using System.Collections.Generic;

namespace AgroTemp.Domain.DTO.Job.JobPost
{
    public class JobDiscoveryDTO
    {
        public Guid Id { get; set; }
        public Guid FarmerProfileId { get; set; }
        public string ContactName { get; set; }
        public List<JobSkillRequirementSummaryDTO> JobSkillRequirements { get; set; } = new();
        public Guid FarmId { get; set; }
        public Guid JobCategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public List<JobPostDayDTO> JobPostDays { get; set; } = new();
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int WorkersNeeded { get; set; }
        public int WorkersAccepted { get; set; }
        public int JobTypeId { get; set; }
        public string JobTypeName { get; set; } // "Daily", "PerPlot", "PerJob"
        public decimal WageAmount { get; set; }
        public List<string> Requirements { get; set; } = new List<string>();
        public List<string> Privileges { get; set; } = new List<string>();
        public DateTime PublishedAt { get; set; }
        public bool IsUrgent { get; set; }
        public int StatusId { get; set; }
        public double? DistanceKm { get; set; }
        public decimal FarmerAverageRating { get; set; }
        public string LocationName { get; set; }
        public int SkillsMatchCount { get; set; }
        public bool AllSkillsMatched { get; set; }
        public int AvailablePositions => WorkersNeeded - WorkersAccepted;
        public int? DurationDays { get; set; }
        public bool IsUpcoming { get; set; }
        public int MatchScore { get; set; }
        public int SimilarJobsCompleted { get; set; }
    }
}
