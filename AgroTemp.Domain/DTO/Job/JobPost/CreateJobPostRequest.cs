using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Domain.DTO.Job.JobPost
{
    public class CreateJobPostRequest
    {
    public List<Guid> JobSkillRequirementIds { get; set; } = new();
    public Guid FarmId { get; set; }
    public Guid JobCategoryId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal EstimatedHours { get; set; }
    public int WorkersNeeded { get; set; }
    public int WorkersAccepted { get; set; }
    public int WageTypeId { get; set; }
    public decimal WageAmount { get; set; }
    public int PaymentMethodId { get; set; }
    public string GenderPreference { get; set; }
    public DateTime PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsUrgent { get; set; }
    public int StatusId { get; set; }
    }
}
