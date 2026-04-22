using AgroTemp.Domain.DTO.DisputeReport;

namespace AgroTemp.Service.Interfaces;

public interface IDisputeReportService
{
    Task<CustomDisputeReportDTO> GetAllDisputesAsync();
    Task<DisputeReportDTO?> GetDisputeByIdAsync(Guid id, Guid currentUserId, bool isAdmin);
    Task<DisputeReportDTO> CreateDisputeAsync(Guid currentUserId, CreateDisputeReportRequest request);
    Task<DisputeReportDTO?> UpdateDisputeAsync(Guid id, Guid currentUserId, UpdateDisputeReportRequest request);
    Task<bool> DeleteDisputeAsync(Guid id, Guid currentUserId, bool isAdmin);
    Task<DisputeReportDTO?> ReviewDisputeAsync(Guid id, Guid adminUserId, ReviewDisputeReportRequest request);
    Task<DisputeReportDTO?> ResolveDisputeAsync(Guid id, Guid adminUserId, ResolveDisputeRequest request);
    Task<List<DisputeReportDTO>> GetMyDisputesAsync(Guid currentUserId);

    // Comments
    Task<List<DisputeReportCommentDTO>> GetDisputeCommentsAsync(Guid disputeId, Guid currentUserId, bool isAdmin);
    Task<DisputeReportCommentDTO> AddDisputeCommentAsync(Guid disputeId, Guid currentUserId, bool isAdmin, CreateDisputeReportCommentRequest request);
}
    