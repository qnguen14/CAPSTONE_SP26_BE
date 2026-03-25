using AgroTemp.Domain.DTO.DisputeReport;

namespace AgroTemp.Service.Interfaces;

public interface IDisputeReportService
{
    Task<List<DisputeReportDTO>> GetAllDisputesAsync();
    Task<DisputeReportDTO?> GetDisputeByIdAsync(Guid id, Guid currentUserId, bool isAdmin);
    Task<DisputeReportDTO> CreateDisputeAsync(Guid currentUserId, CreateDisputeReportRequest request);
    Task<DisputeReportDTO?> UpdateDisputeAsync(Guid id, Guid currentUserId, UpdateDisputeReportRequest request);
    Task<bool> DeleteDisputeAsync(Guid id, Guid currentUserId, bool isAdmin);
    Task<DisputeReportDTO?> ReviewDisputeAsync(Guid id, Guid adminUserId, ReviewDisputeReportRequest request);
    Task<DisputeReportDTO?> ResolveDisputeAsync(Guid id, Guid adminUserId, ResolveDisputeRequest request);
    Task<List<DisputeReportDTO>> GetMyDisputesAsync(Guid currentUserId);
}
    