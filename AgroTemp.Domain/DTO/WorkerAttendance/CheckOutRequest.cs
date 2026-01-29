using System;
using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.WorkerAttendance
{
    public class CheckOutRequest
    {
        [Required]
        public Guid AttendanceId { get; set; }

        [Required]
        public DateTime CheckOutTime { get; set; }

        [StringLength(500)]
        public string? CheckOutNotes { get; set; }
    }
}
