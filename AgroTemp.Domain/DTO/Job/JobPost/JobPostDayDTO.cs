using System;

namespace AgroTemp.Domain.DTO.Job.JobPost
{
    public class JobPostDayRequest
    {
        public DateOnly WorkDate { get; set; }
        public int WorkersNeeded { get; set; }
    }

    public class JobPostDayDTO
    {
        public DateOnly WorkDate { get; set; }
        public int WorkersNeeded { get; set; }
        public int WorkersAccepted { get; set; }
    }
}
