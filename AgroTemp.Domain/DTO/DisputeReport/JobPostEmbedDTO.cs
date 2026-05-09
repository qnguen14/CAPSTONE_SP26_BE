namespace AgroTemp.Domain.DTO.DisputeReport;

/// <summary>
/// Lightweight job-post card embedded inside a dispute comment.
/// </summary>
public class JobPostEmbedDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Address { get; set; }
    public decimal WageAmount { get; set; }
    public int StatusId { get; set; }
    public string? StatusName { get; set; }
    public string? FarmerName { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsUrgent { get; set; }
}
