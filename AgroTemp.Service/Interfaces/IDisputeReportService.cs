using AgroTemp.Domain.DTO.DisputeReport;

namespace AgroTemp.Service.Interfaces;

public interface IDisputeReportService
{
    Task<List<DisputeReportDTO>> GetAllDisputesAsync();
    Task<DisputeReportDTO?> GetDisputeByIdAsync(Guid id);
    Task<DisputeReportDTO> CreateDisputeAsync(CreateDisputeReportRequest request);
    Task<DisputeReportDTO?> UpdateDisputeAsync(Guid id, UpdateDisputeReportRequest request);
    Task<bool> DeleteDisputeAsync(Guid id);
}
