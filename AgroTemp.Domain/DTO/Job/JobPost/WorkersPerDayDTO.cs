namespace AgroTemp.Domain.DTO.Job.JobPost
{
    public class WorkerSummaryDTO
    {
        public Guid WorkerId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class WorkersPerDayDTO
    {
        public DateOnly Date { get; set; }
        public int AcceptedWorkerCount { get; set; }
        public List<WorkerSummaryDTO> Workers { get; set; } = new();
    }
}
