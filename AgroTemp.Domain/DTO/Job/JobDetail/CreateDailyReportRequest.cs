using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.DTO.Job.JobDetail
{
    public class CreateDailyReportRequest
    {
        public string WorkerDescription { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}