using System;

namespace AgroTemp.Domain.DTO.WorkerAttendance
{
    public class WorkerAttendanceDTO
    {
        public Guid Id { get; set; }
        public Guid JobApplicationId { get; set; }
        public DateTime WorkDate { get; set; }
        public DateTime CheckInTime { get; set; }
        public string? CheckInNotes { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string? CheckOutNotes { get; set; }
        public decimal? TotalHoursWorked { get; set; }
        public bool IsApproved { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
