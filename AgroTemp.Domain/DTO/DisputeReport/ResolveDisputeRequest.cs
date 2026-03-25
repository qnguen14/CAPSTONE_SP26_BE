using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class ResolveDisputeRequest
{
    [Required]
    public bool IsResolved { get; set; }

    [Required]
    public string AdminNote { get; set; } = string.Empty;
}