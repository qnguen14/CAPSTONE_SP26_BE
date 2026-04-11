using System;

namespace AgroTemp.Domain.DTO.Job.JobDetail
{
    public class JobAttachmentDTO
    {
        public Guid Id { get; set; }

        public Guid JobDetailId { get; set; }

        public string CloudinaryPublicId { get; set; }

        public string FileUrl { get; set; }

        public string? Format { get; set; }

        public long? FileSize { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
