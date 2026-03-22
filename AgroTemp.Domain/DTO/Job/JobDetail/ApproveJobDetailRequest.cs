using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.DTO.Job.JobDetail
{
    public class ApproveJobDetailRequest
    {
        public int FarmerApprovedPercent { get; set; }

        public string FarmerFeedback { get; set; }
    }
}