using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.DTO.Job.JobApplication
{
    public class RespondJobApplicationRequest
    {
        public int StatusId { get; set; }
        public DateTime RespondedAt { get; set; }
        public string ResponseMessage { get; set; } = string.Empty;
    }
}
