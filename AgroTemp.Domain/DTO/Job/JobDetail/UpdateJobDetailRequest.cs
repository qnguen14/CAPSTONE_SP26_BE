using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.DTO.Job.JobDetail
{
    public class UpdateJobDetailRequest
    {

        public Guid JobApplicationId { get; set; }

        public Guid JobPostId { get; set; }

        public Guid WorkerId { get; set; }

        public int StatusId { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool WorkerCheckedIn { get; set; }

        public bool FarmerConfirmedAttendance { get; set; }

        public decimal? TotalHoursWorked { get; set; }

        public decimal? TotalAmountDue { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
