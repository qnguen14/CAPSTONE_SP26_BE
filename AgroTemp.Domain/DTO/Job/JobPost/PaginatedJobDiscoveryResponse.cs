using System;
using System.Collections.Generic;

namespace AgroTemp.Domain.DTO.Job.JobPost
{
    public class PaginatedJobDiscoveryResponse
    {
        public List<JobDiscoveryDTO> Jobs { get; set; } = new List<JobDiscoveryDTO>();
        
        public int TotalCount { get; set; }
        
        public int PageNumber { get; set; }
        
        public int PageSize { get; set; }
        
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        
        public bool HasPreviousPage => PageNumber > 1;
        
        public bool HasNextPage => PageNumber < TotalPages;
        
        public string Message { get; set; } = "Job results retrieved successfully";
    }
}
