using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.DisputeReport;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AgroTemp.Service.Implements;

public class DisputeReportService : BaseService<DisputeReport>, IDisputeReportService
{
    public DisputeReportService(
        IUnitOfWork<AgroTempDbContext> unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<List<DisputeReportDTO>> GetAllDisputesAsync()
    {
        var disputes = await _unitOfWork.GetRepository<DisputeReport>()
                .GetListAsync(
                    predicate: null,
                    include: q => q
                        .Include(d => d.Farmer)
                        .Include(d => d.Worker)
                        .Include(d => d.JobPost)
                        .Include(d => d.ResolvedBy),
                    orderBy: d => d.OrderByDescending(x => x.CreatedAt));

        return disputes == null || !disputes.Any()
            ? new List<DisputeReportDTO>() : _mapper.DisputeReportsToDisputeReportDtos(disputes);
    }

    public async Task<DisputeReportDTO?> GetDisputeByIdAsync(Guid id, Guid currentUserId, bool isAdmin)
    {
        var dispute = await _unitOfWork.GetRepository<DisputeReport>()
            .FirstOrDefaultAsync(
                predicate: d => d.Id == id,
                include: q => q
                    .Include(d => d.Farmer)
                    .Include(d => d.Worker)
                    .Include(d => d.JobPost)
                    .Include(d => d.ResolvedBy));

        if (dispute == null)
        {
            return null;
        }

        if(!isAdmin)
        {
            await EnsureIsOwnerAsync(dispute, currentUserId);
        }

        return _mapper.DisputeReportToDisputeReportDto(dispute);
    }

    public async Task<DisputeReportDTO> CreateDisputeAsync(Guid currentUserId, CreateDisputeReportRequest request)
    {
        var JobPost = await _unitOfWork.GetRepository<JobPost>()
            .FirstOrDefaultAsync(predicate: j => j.Id == request.JobPostId);

        if(JobPost == null)
        {
            throw new KeyNotFoundException("Job post not found");
        }

        var farmer = await _unitOfWork.GetRepository<Farmer>()
            .FirstOrDefaultAsync(predicate: f => f.UserId == currentUserId);

        var worker = await _unitOfWork.GetRepository<Worker>()
            .FirstOrDefaultAsync(predicate: w => w.UserId == currentUserId);
        
        var isJobOwnerFarmer = farmer != null && JobPost.FarmerId == farmer.Id;
        var isWorkerApplied = false;

        if(worker != null)
        {
            isWorkerApplied = await _unitOfWork.GetRepository<JobApplication>()
                .FirstOrDefaultAsync(predicate: ja => ja.JobPostId == request.JobPostId && ja.WorkerId == worker.Id) != null;
        }

        if(!isJobOwnerFarmer && !isWorkerApplied)
        {
            throw new UnauthorizedAccessException("You are not allowed to create a dispute report for this job post");
        }

        var dispute = _mapper.CreateDisputeReportRequestToDisputeReport(request);
        if(dispute.Id == Guid.Empty)
        {
            dispute.Id = Guid.NewGuid();
        }

        dispute.CreatedAt = DateTime.UtcNow;
        dispute.StatusId = (int)DisputeStatus.Pending;
        dispute.ResolvedAt = null;
        dispute.ResolvedById = null;
        dispute.FarmerId = farmer?.Id;
        dispute.WorkerId = worker?.Id;

        await _unitOfWork.GetRepository<DisputeReport>().InsertAsync(dispute);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.GetRepository<DisputeReport>()
            .FirstOrDefaultAsync(
                predicate: d => d.Id == dispute.Id,
                include: q => q
                    .Include(d => d.Farmer)
                    .Include(d => d.Worker)
                    .Include(d => d.JobPost)
                    .Include(d => d.ResolvedBy));

        return _mapper.DisputeReportToDisputeReportDto(created!);
    }

    public async Task<DisputeReportDTO?> UpdateDisputeAsync(Guid id, Guid currentUserId, UpdateDisputeReportRequest request)
    {
        var dispute = await _unitOfWork.GetRepository<DisputeReport>()
            .FirstOrDefaultAsync(predicate: d => d.Id == id);

        if(dispute == null)
        {
            return null;
        }

        await EnsureIsOwnerAsync(dispute, currentUserId);

        if(dispute.StatusId != (int)DisputeStatus.Pending)
        {
            throw new InvalidOperationException("Only pending disputes can be updated by user.");
        }

        if (!string.IsNullOrWhiteSpace(request.Reason)) dispute.Reason = request.Reason;
        
        if(request.Description != null)
        {
            dispute.Description = request.Description;
        }
        
        if(request.EvidenceUrl != null)
        {
            dispute.EvidenceUrl = request.EvidenceUrl;
        }

        if(request.DisputeTypeId.HasValue)
        {
            dispute.DisputeTypeId = request.DisputeTypeId.Value;
        }

        _unitOfWork.GetRepository<DisputeReport>().UpdateAsync(dispute);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.DisputeReportToDisputeReportDto(dispute);
    }

    public async Task<bool> DeleteDisputeAsync(Guid id, Guid currentUserId, bool isAdmin)
    {
        var dispute = await _unitOfWork.GetRepository<DisputeReport>()
            .FirstOrDefaultAsync(predicate: d => d.Id == id);

        if(dispute == null)
        {
            return false;
        }

        if(!isAdmin)
        {
            await EnsureIsOwnerAsync(dispute, currentUserId);

            if(dispute.StatusId != (int)DisputeStatus.Pending)
            {
                throw new InvalidOperationException("Only pending disputes can be deleted by user.");
            }
        }

        _unitOfWork.GetRepository<DisputeReport>().DeleteAsync(dispute);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<List<DisputeReportDTO>> GetMyDisputesAsync(Guid currentUserId)
    {
        var farmer = await _unitOfWork.GetRepository<Farmer>()
            .FirstOrDefaultAsync(predicate: f => f.UserId == currentUserId);

        var worker = await _unitOfWork.GetRepository<Worker>()
            .FirstOrDefaultAsync(predicate: w => w.UserId == currentUserId);

        var farmerId = farmer?.Id;
        var workerId = worker?.Id;

        var disputes = await _unitOfWork.GetRepository<DisputeReport>()
            .GetListAsync(
                predicate: d => 
                    (farmerId.HasValue && d.FarmerId == farmerId.Value) ||
                    (workerId.HasValue && d.WorkerId == workerId.Value),
                include: q => q
                    .Include(d => d.Farmer)
                    .Include(d => d.Worker)
                    .Include(d => d.JobPost)
                    .Include(d => d.ResolvedBy),
                orderBy: d => d.OrderByDescending(x => x.CreatedAt));

        return disputes == null || !disputes.Any()
            ? new List<DisputeReportDTO>() : _mapper.DisputeReportsToDisputeReportDtos(disputes);
    }

    public async Task<DisputeReportDTO?> ReviewDisputeAsync(Guid id, Guid adminUserId, ReviewDisputeReportRequest request)
    {
        var dispute = await _unitOfWork.GetRepository<DisputeReport>()
            .FirstOrDefaultAsync(predicate: d => d.Id == id);

        if(dispute == null)
        {
            return null;
        }

        if(dispute.StatusId != (int)DisputeStatus.Pending)
        {
            throw new InvalidOperationException("Only pending disputes can be reviewed by admin.");
        }

        dispute.StatusId = (int)DisputeStatus.UnderReview;
        dispute.AdminNote = request.AdminNote;
        dispute.ResolvedById = adminUserId;

        _unitOfWork.GetRepository<DisputeReport>().UpdateAsync(dispute);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.DisputeReportToDisputeReportDto(dispute);
    }

    public async Task<DisputeReportDTO?> ResolveDisputeAsync(Guid id, Guid adminUserId, ResolveDisputeRequest request)
    {
        var dispute = await _unitOfWork.GetRepository<DisputeReport>()
            .FirstOrDefaultAsync(predicate: d => d.Id == id);
            
        if(dispute == null)
        {
            return null;
        }

        if(dispute.StatusId != (int)DisputeStatus.UnderReview)
        {
            throw new InvalidOperationException("Only under review disputes can be resolved by admin.");
        }

        dispute.StatusId = request.IsResolved
            ? (int)DisputeStatus.Resolved : (int)DisputeStatus.Rejected;

        dispute.AdminNote = request.AdminNote;
        dispute.ResolvedById = adminUserId;
        dispute.ResolvedAt = DateTime.UtcNow;

        _unitOfWork.GetRepository<DisputeReport>().UpdateAsync(dispute);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.DisputeReportToDisputeReportDto(dispute);    
    }

    private async Task EnsureIsOwnerAsync(DisputeReport dispute, Guid currentUserId)
    {
        var farmer = await _unitOfWork.GetRepository<Farmer>()
            .FirstOrDefaultAsync(predicate: f => f.UserId == currentUserId);
        
        var worker = await _unitOfWork.GetRepository<Worker>()
            .FirstOrDefaultAsync(predicate: w => w.UserId == currentUserId);
        
        var isOwner = (farmer != null && dispute.FarmerId == farmer.Id) ||
                      (worker != null && dispute.WorkerId == worker.Id);

        if(!isOwner)
        {
            throw new UnauthorizedAccessException("You are not allowed to access this dispute report.");
        }
    }
}
