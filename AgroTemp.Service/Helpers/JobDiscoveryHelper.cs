using AgroTemp.Domain.DTO.Job.JobPost;
using AgroTemp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Service.Helpers
{
    public class JobDiscoveryHelper
    {
        public static List<JobPost> ApplyJobFilters(List<JobPost> jobs, JobSearchFilterRequest filter)
        {
            var filtered = jobs.Where(jp => jp.StatusId == (int)JobPostStatus.Published).ToList();

            // Filter by distance
            if (filter.WorkerLatitude.HasValue && filter.WorkerLongitude.HasValue &&
                filter.MaxDistanceKm.HasValue &&
                (filter.WorkerLatitude.Value != 0 || filter.WorkerLongitude.Value != 0))
            {
                filtered = filtered.Where(jp => jp.Farm != null &&
                    DistanceCalculator.GetDistanceInKilometers(
                        filter.WorkerLatitude.Value, filter.WorkerLongitude.Value,
                        jp.Farm.Latitude, jp.Farm.Longitude) <= filter.MaxDistanceKm.Value).ToList();
            }

            // Filter by wage
            if (filter.MinWageAmount.HasValue)
            {
                filtered = filtered.Where(jp => jp.WageAmount >= filter.MinWageAmount.Value).ToList();
            }

            if (filter.MaxWageAmount.HasValue)
            {
                filtered = filtered.Where(jp => jp.WageAmount <= filter.MaxWageAmount.Value).ToList();
            }

            // Filter by job type
            if (filter.JobTypeId.HasValue)
            {
                filtered = filtered.Where(jp => jp.JobTypeId == filter.JobTypeId.Value).ToList();
            }

            // Filter by category
            if (filter.JobCategoryId.HasValue)
            {
                filtered = filtered.Where(jp => jp.JobCategoryId == filter.JobCategoryId.Value).ToList();
            }

            // Filter by keyword
            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.ToLower();
                filtered = filtered.Where(jp =>
                    jp.Title.ToLower().Contains(keyword) ||
                    jp.Description.ToLower().Contains(keyword) ||
                    jp.JobSkillRequirements.Any(jsr => jsr.Skill.Name.ToLower().Contains(keyword))).ToList();
            }

            // Filter by skills
            if (filter.RequiredSkills?.Any() == true)
            {
                filtered = filtered.Where(jp =>
                    filter.RequiredSkills.Any(s => jp.JobSkillRequirements.Any(jsr => jsr.Skill.Name == s))).ToList();
            }

            // Filter by date
            if (!string.IsNullOrWhiteSpace(filter.DateFilter))
            {
                var now = DateTime.UtcNow;
                DateTime? dateStart = null;
                DateTime? dateEnd = null;

                switch (filter.DateFilter.ToLower())
                {
                    case "today" or "hôm nay":
                        dateStart = now.Date;
                        dateEnd = now.Date.AddDays(1).AddTicks(-1);
                        break;
                    case "tomorrow" or "ngày mai":
                        dateStart = now.Date.AddDays(1);
                        dateEnd = now.Date.AddDays(2).AddTicks(-1);
                        break;
                    case "upcoming":
                        dateStart = now.Date;
                        dateEnd = now.Date.AddDays(30).AddTicks(-1);
                        break;
                }

                if (dateStart.HasValue && dateEnd.HasValue)
                {
                    filtered = filtered.Where(jp =>
                        (jp.StartDate?.ToDateTime(TimeOnly.MinValue) >= dateStart && jp.StartDate?.ToDateTime(TimeOnly.MinValue) <= dateEnd) ||
                        (jp.SelectedDays.Any(d => d.ToDateTime(TimeOnly.MinValue) >= dateStart && d.ToDateTime(TimeOnly.MinValue) <= dateEnd))).ToList();
                }
            }

            // Filter by custom date range
            if (filter.StartDateFrom.HasValue)
            {
                filtered = filtered.Where(jp => jp.StartDate?.ToDateTime(TimeOnly.MinValue) >= filter.StartDateFrom.Value).ToList();
            }

            if (filter.StartDateTo.HasValue)
            {
                filtered = filtered.Where(jp => jp.StartDate?.ToDateTime(TimeOnly.MinValue) <= filter.StartDateTo.Value).ToList();
            }

            // Filter by duration
            if (!string.IsNullOrWhiteSpace(filter.DurationType))
            {
                if (filter.DurationType.ToLower() == "hours")
                {
                    // Single day jobs
                    filtered = filtered.Where(jp => jp.StartDate.HasValue && jp.EndDate.HasValue &&
                        jp.EndDate.Value.DayNumber == jp.StartDate.Value.DayNumber).ToList();
                }
                else if (filter.DurationType.ToLower() == "days")
                {
                    // Multi-day jobs
                    filtered = filtered.Where(jp => jp.StartDate.HasValue && jp.EndDate.HasValue &&
                        jp.EndDate.Value.DayNumber > jp.StartDate.Value.DayNumber).ToList();
                }
            }

            // Filter urgent
            if (filter.OnlyUrgent.HasValue && filter.OnlyUrgent.Value)
            {
                filtered = filtered.Where(jp => jp.IsUrgent).ToList();
            }

            return filtered;
        }

        public static List<JobDiscoveryDTO> ApplyJobSorting(List<JobDiscoveryDTO> jobs, string sortBy)
        {
            return (sortBy?.ToLower()) switch
            {
                "salary" => jobs.OrderByDescending(j => j.WageAmount).ToList(),
                "date" => jobs.OrderBy(j => j.StartDate).ToList(),
                "rating" => jobs.OrderByDescending(j => j.FarmerAverageRating).ToList(),
                "newest" => jobs.OrderByDescending(j => j.PublishedAt).ToList(),
                "distance" => jobs.OrderBy(j => j.DistanceKm ?? double.MaxValue).ToList(),
                _ => jobs.OrderBy(j => j.DistanceKm ?? double.MaxValue).ToList()
            };
        }

        public static void ApplyDiscoveryCalculations(JobDiscoveryDTO dto, JobSearchFilterRequest filter)
        {
            if (dto == null) return;

            // Calculate distance if worker location provided
            if (filter?.WorkerLatitude.HasValue == true && filter.WorkerLongitude.HasValue)
            {
                // Note: The entity should have been loaded with Farm relationship
                // If not, distance stays null
                // In actual implementation, you might need to reload entity to get Farm
            }

            // Calculate match score
            dto.MatchScore = CalculateMatchScore(filter, dto);
        }

        private static int CalculateMatchScore(JobSearchFilterRequest filter, JobDiscoveryDTO job)
        {
            if (filter == null) return 50; // Neutral score

            int score = 50;

            // Bonus for skills match
            if (filter.RequiredSkills?.Any() == true)
            {
                var matchedSkills = filter.RequiredSkills
                    .Count(s => job.JobSkillRequirements?.Exists(jsr => jsr.Name == s) ?? false);
                score += matchedSkills * 10;
            }

            // Bonus for wage in range
            if (filter.MinWageAmount.HasValue && job.WageAmount >= filter.MinWageAmount.Value)
            {
                score += 10;
            }

            // Bonus for nearby
            if (filter.WorkerLatitude.HasValue && filter.WorkerLongitude.HasValue && (job.DistanceKm.HasValue))
            {
                if (job.DistanceKm <= 5) score += 20;
                else if (job.DistanceKm <= 10) score += 10;
            }

            // Bonus for urgent
            if (job.IsUrgent) score += 10;

            // Cap score at 100
            return Math.Min(score, 100);
        }
    }
}
