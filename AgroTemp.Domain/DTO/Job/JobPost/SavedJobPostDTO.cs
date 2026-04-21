namespace AgroTemp.Domain.DTO.Job.JobPost
{
    public class SavedJobPostDTO
    {
        public Guid Id { get; set; }
        public Guid WorkerId { get; set; }
        public DateTime SavedAt { get; set; }
        public JobPostDTO JobPost { get; set; }
    }
}
