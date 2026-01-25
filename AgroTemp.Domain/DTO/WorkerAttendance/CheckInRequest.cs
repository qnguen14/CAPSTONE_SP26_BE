using System;
using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.WorkerAttendance
{
    public class CheckInRequest
    {
        [Required]
        public Guid JobApplicationId { get; set; }

        [Required]
        public DateTime CheckInTime { get; set; }

        [StringLength(500)]
        public string? CheckInNotes { get; set; }
    }
}
