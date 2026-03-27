using System;
using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.WorkerAttendance
{
    public class ApproveAttendanceRequest
    {
        [Required]
        public Guid AttendanceId { get; set; }

        public decimal? AdjustedHours { get; set; }
    }
}
