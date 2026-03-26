using System;
using System.Collections.Generic;

namespace AgroTemp.Domain.DTO.Job.JobPost
{
    public class JobSearchFilterRequest
    {
        public decimal? WorkerLatitude { get; set; }
        public decimal? WorkerLongitude { get; set; }
        public double? MaxDistanceKm { get; set; } = 20;
        public decimal? MinWageAmount { get; set; }
        public decimal? MaxWageAmount { get; set; }
        public int? JobTypeId { get; set; }
        public Guid? JobCategoryId { get; set; }
        public string? SearchKeyword { get; set; }
        public List<string>? RequiredSkills { get; set; }
        public string? DateFilter { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public string? DurationType { get; set; }
        public string? PaymentMethod { get; set; }
        public bool? OnlyUrgent { get; set; }
        public decimal? MinWorkerRating { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "distance";
    }
}
