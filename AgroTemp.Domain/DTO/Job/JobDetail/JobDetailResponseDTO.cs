using AgroTemp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.DTO.Job.JobDetail
{
    public class JobDetailResponseDTO
    {
        public Guid Id { get; set; }

        public Guid JobApplicationId { get; set; }

        public Guid JobPostId { get; set; }

        public Guid WorkerId { get; set; }

        public int StatusId { get; set; }

        public DateTime? WorkDate { get; set; }

        public string WorkerDescription { get; set; }

        public string FarmerFeedback { get; set; }

        public int? FarmerApprovedPercent { get; set; }

        public decimal JobPrice { get; set; }

        public decimal? WorkerPaymentAmount { get; set; }

        public decimal? RefundAmount { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<JobAttachmentDTO> Attachments { get; set; } = new List<JobAttachmentDTO>();
    }
}