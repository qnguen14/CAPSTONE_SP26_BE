using System.ComponentModel.DataAnnotations;

namespace AgroTemp.Domain.DTO.DisputeReport;

public class UpdateDisputeStatusRequest
{
    [Required]
    public int StatusId { get; set; }
}
