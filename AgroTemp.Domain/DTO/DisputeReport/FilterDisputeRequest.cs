namespace AgroTemp.Domain.DTO.DisputeReport;

public class FilterDisputeRequest
{
    public string? JobPostName { get; set; }
    public int? DisputeTypeId { get; set; }
    public int? StatusId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
