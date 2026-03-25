using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class ReviewDisputeReportRequest
{
    [Required]
    public string AdminNote { get; set; } = string.Empty;
}