using System;

namespace AgroTemp.Domain.DTO.Job.JobPost
{
    public class SimpleUserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
    }

    public class AdminJobPostItemDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public SimpleUserDto Farmer { get; set; }
        public SimpleUserDto Worker { get; set; }
        public string Status { get; set; }
        public decimal Salary { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
