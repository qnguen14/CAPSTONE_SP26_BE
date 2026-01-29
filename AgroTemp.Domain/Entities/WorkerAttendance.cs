using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.Entities
{
    [Table("Worker_Attendance")]
    public class WorkerAttendance
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(JobApplication))]
        [Column("job_application_id")]
        public Guid JobApplicationId { get; set; }
        public virtual JobApplication JobApplication { get; set; }

        [Required]
        [Column("work_date")]
        public DateTime WorkDate { get; set; }

        [Required]
        [Column("check_in_time")]
        public DateTime CheckInTime { get; set; }

        [Column("check_in_notes")]
        [StringLength(500)]
        public string? CheckInNotes { get; set; }

        [Column("check_out_time")]
        public DateTime? CheckOutTime { get; set; }

        [Column("check_out_notes")]
        [StringLength(500)]
        public string? CheckOutNotes { get; set; }

        [Column("total_hours_worked")]
        public decimal? TotalHoursWorked { get; set; }

        [Column("is_approved")]
        public bool IsApproved { get; set; }

        [Column("approved_by")]
        public Guid? ApprovedBy { get; set; }

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
