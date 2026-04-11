using System.ComponentModel.DataAnnotations;
using AgroTemp.Domain.Entities;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class ResolveDisputeRequest
{
    [Required]
    public bool IsResolved { get; set; }

    [Required]
    public string AdminNote { get; set; } = string.Empty;

    // Who gets penalized:
    //   None (0)     = both sides are acceptable, no ban
    //   Reporter (1) = the reporter was wrong → ban reporter
    //   Accused (2)  = the accused was wrong  → ban accused
    [Required]
    [Range(0, 2, ErrorMessage = "PenaltyTarget must be 0 (None), 1 (Reporter), or 2 (Accused)")]
    public PenaltyTarget PenaltyTarget { get; set; } = PenaltyTarget.None;
}