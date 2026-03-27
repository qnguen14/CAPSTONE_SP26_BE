using AgroTemp.Domain.DTO.Job.JobPost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.DTO.Job.JobApplication
{
    public class JobApplicationDTO
    {
        public Guid Id { get; set; }
        
        public Guid JobPostId { get; set; }
        
        public JobPostDTO? JobPost { get; set; }

        public WorkerProfileDTO? Worker { get; set; }
        
        public int StatusId { get; set; }
        
        public string? CoverLetter { get; set; }
        
        public DateTime AppliedAt { get; set; }
        
        public DateTime? RespondedAt { get; set; }
        
        public string? ResponseMessage { get; set; }

        public List<DateTime>? WorkDates { get; set; } = new List<DateTime>();
        public string? LocationName { get; set; }
    }
}
