namespace AgroTemp.Domain.DTO.DisputeReport;

public class DisputeStatusSummaryDTO
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
}
