using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.DTO.Job.JobApplication
{
    public class UpdateJobApplicationRequest
    {
        public Guid Id { get; set; }

        public Guid JobPostId { get; set; }

        public Guid WorkerId { get; set; }

        public int StatusId { get; set; }

        public string? CoverLetter { get; set; }

        public DateTime AppliedAt { get; set; }

        public DateTime RespondedAt { get; set; }

        public string? ResponseMessage { get; set; }
    }
}
