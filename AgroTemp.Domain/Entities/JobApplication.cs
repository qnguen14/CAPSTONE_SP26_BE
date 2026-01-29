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
        [ForeignKey(nameof(WorkerProfile))]
        [Column("worker_profile_id")]
        public Guid WorkerProfileId { get; set; }   
        public virtual WorkerProfile WorkerProfile { get; set; }

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
        public DateTime RespondedAt { get; set; }

        [Column("response_message")]
        public string? ResponseMessage { get; set; }

        public virtual ICollection<WorkerAttendance> WorkerAttendances { get; set; } = new List<WorkerAttendance>();
        public virtual ICollection<JobAssignment> JobAssignments { get; set; } = new List<JobAssignment>();
    }
}
