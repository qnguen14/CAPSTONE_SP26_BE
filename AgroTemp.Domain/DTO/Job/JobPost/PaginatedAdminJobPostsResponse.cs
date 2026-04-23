using System;
using System.Collections.Generic;

namespace AgroTemp.Domain.DTO.Job.JobPost
{
    public class AdminJobPostSummaryDto
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Completed { get; set; }
        public double CompletionRate { get; set; }
    }

    public class PaginatedAdminJobPostsResponse
    {
        public List<AdminJobPostItemDTO> Data { get; set; } = new();
        public AdminJobPostSummaryDto Summary { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
