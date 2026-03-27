using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.Entities
{
    public enum ApplicationStatus
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3,
        Cancelled = 4
    }

    [Table("Job_Application")]
    public class JobApplication
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(JobPost))]
        [Column("job_post_id")]
        public Guid JobPostId { get; set; }
        public virtual JobPost JobPost { get; set; }

        [Required]
        [ForeignKey(nameof(Worker))]
        [Column("worker_id")]
        public Guid WorkerId { get; set; }   
        public virtual Worker Worker { get; set; }

        [Required]
        [Column("status")]
        public int StatusId { get; set; }

        [NotMapped]
        public ApplicationStatus Status
        {
            get => (ApplicationStatus)StatusId;
            set => StatusId = (int)value;
        }

        [Column("cover_letter")]
        public string? CoverLetter { get; set; }

        [Required]
        [Column("applied_at")]
        public DateTime AppliedAt { get; set; }

        [Column("responded_at")]
        public DateTime? RespondedAt { get; set; }

        [Column("response_message")]
        public string? ResponseMessage { get; set; }

        [Column("work_dates")]
        public List<DateTime>? WorkDates { get; set; }

        public virtual ICollection<JobDetail> JobDetails { get; set; } = new List<JobDetail>();
    }
}
