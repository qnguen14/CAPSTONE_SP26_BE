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
        var jobPost = await _unitOfWork.GetRepository<JobPost>()
            .FirstOrDefaultAsync(predicate: j => j.Id == request.JobPostId);

        if(jobPost == null)
        {
            throw new KeyNotFoundException("Job post not found");
        }

        var reporter_farmer = await _unitOfWork.GetRepository<Farmer>()
            .FirstOrDefaultAsync(predicate: f => f.UserId == currentUserId);

        var reporter_worker = await _unitOfWork.GetRepository<Worker>()
            .FirstOrDefaultAsync(predicate: w => w.UserId == currentUserId);
        
        var isJobOwnerFarmer = reporter_farmer != null && jobPost.FarmerId == reporter_farmer.Id;
        var isWorkerApplied = false;

        if(reporter_worker != null)
        {
            isWorkerApplied = await _unitOfWork.GetRepository<JobApplication>()
                .FirstOrDefaultAsync(predicate: ja => ja.JobPostId == request.JobPostId && ja.WorkerId == reporter_worker.Id) != null;
        }

        if(!isJobOwnerFarmer && !isWorkerApplied)
        {
            throw new UnauthorizedAccessException("You are not allowed to create a dispute report for this job post");
        }


        Guid? accusedUserId = null;
        Guid? accusedFarmerId = null;
        Guid? accusedWorkerId = null;

        if(isJobOwnerFarmer)
        {
            if(request.WorkerId.HasValue)
            {
                var accusedWorker = await _unitOfWork.GetRepository<Worker>()
                    .FirstOrDefaultAsync(predicate: w => w.Id == request.WorkerId.Value);
                if(accusedWorker == null)
                    throw new KeyNotFoundException("Accused worker not found");
                accusedUserId = accusedWorker.UserId;
                accusedWorkerId = accusedWorker.Id;
            }
            else
            {
                var application = await _unitOfWork.GetRepository<JobApplication>()
                    .FirstOrDefaultAsync(predicate: ja => ja.JobPostId == request.JobPostId
                        && ja.StatusId == (int)ApplicationStatus.Accepted);
                if(application != null)
                {
                    var accusedWorker = await _unitOfWork.GetRepository<Worker>()
                        .FirstOrDefaultAsync(predicate: w => w.Id == application.WorkerId);
                    if(accusedWorker != null)
                    {
                        accusedUserId = accusedWorker.UserId;
                        accusedWorkerId = accusedWorker.Id;
                    }
                }
            }
        }
        else
        {
            var accusedFarmer = await _unitOfWork.GetRepository<Farmer>()
                .FirstOrDefaultAsync(predicate: f => f.Id == jobPost.FarmerId);
            if(accusedFarmer != null)
            {
                accusedUserId = accusedFarmer.UserId;
                accusedFarmerId = accusedFarmer.Id;
            }
        }

        var dispute = _mapper.CreateDisputeReportRequestToDisputeReport(request);
        if(dispute.Id == Guid.Empty)
        {
            dispute.Id = Guid.NewGuid();
        }

        dispute.CreatedAt = DateTime.UtcNow;
        dispute.StatusId = (int)DisputeStatus.Pending;
        dispute.PenaltyTargetId = (int)PenaltyTarget.None;
        dispute.ResolvedAt = null;
        dispute.ResolvedById = null;

        dispute.FarmerId = reporter_farmer?.Id;
        dispute.WorkerId = reporter_worker?.Id;
        dispute.ReporterUserId = currentUserId;

        dispute.AccusedUserId = accusedUserId;
        if(isJobOwnerFarmer && accusedWorkerId.HasValue)
        {
            dispute.WorkerId = accusedWorkerId;
        }
        else if(!isJobOwnerFarmer && accusedFarmerId.HasValue)
        {
            dispute.FarmerId = accusedFarmerId;
        }

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
        dispute.PenaltyTargetId = (int)request.PenaltyTarget;


        if(request.PenaltyTarget != PenaltyTarget.None)
        {
            Guid? userToBanId = request.PenaltyTarget == PenaltyTarget.Reporter
                ? dispute.ReporterUserId
                : dispute.AccusedUserId;

            if(userToBanId.HasValue)
            {
                var userToBan = await _unitOfWork.GetRepository<User>()
                    .FirstOrDefaultAsync(predicate: u => u.Id == userToBanId.Value);

                if(userToBan == null)
                    throw new KeyNotFoundException($"User to ban not found (UserId={userToBanId})");

                userToBan.WarningCount += 1;
                userToBan.LastWarnedAt = DateTime.UtcNow;
                if(userToBan.WarningCount >= 2)
                {
                    userToBan.IsActive = false;
                }
                _unitOfWork.GetRepository<User>().UpdateAsync(userToBan);
            }
        }

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

    public async Task<List<DisputeReportCommentDTO>> GetDisputeCommentsAsync(Guid disputeId, Guid currentUserId, bool isAdmin)
    {
        var dispute = await _unitOfWork.GetRepository<DisputeReport>()
            .FirstOrDefaultAsync(predicate: d => d.Id == disputeId);

        if (dispute == null) throw new KeyNotFoundException("Dispute not found");

        if (!isAdmin)
        {
            await EnsureIsOwnerAsync(dispute, currentUserId);
        }

        var comments = await _unitOfWork.Context.Set<DisputeReportComment>()
            .Include(c => c.User)
            .ThenInclude(u => u.Farmer)
            .Include(c => c.User)
            .ThenInclude(u => u.Worker)
            .Where(c => c.DisputeReportId == disputeId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return _mapper.DisputeReportCommentsToDtos(comments);
    }

    public async Task<DisputeReportCommentDTO> AddDisputeCommentAsync(Guid disputeId, Guid currentUserId, bool isAdmin, CreateDisputeReportCommentRequest request)
    {
        var dispute = await _unitOfWork.GetRepository<DisputeReport>()
            .FirstOrDefaultAsync(predicate: d => d.Id == disputeId);

        if (dispute == null) throw new KeyNotFoundException("Dispute not found");

        if (!isAdmin)
        {
            await EnsureIsOwnerAsync(dispute, currentUserId);
        }

        var comment = new DisputeReportComment
        {
            Id = Guid.NewGuid(),
            DisputeReportId = disputeId,
            UserId = currentUserId,
            Content = request.Content,
            AttachmentUrl = request.AttachmentUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Context.Set<DisputeReportComment>().AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        var createdComment = await _unitOfWork.Context.Set<DisputeReportComment>()
            .Include(c => c.User)
            .ThenInclude(u => u.Farmer)
            .Include(c => c.User)
            .ThenInclude(u => u.Worker)
            .FirstOrDefaultAsync(c => c.Id == comment.Id);

        return _mapper.DisputeReportCommentToDto(createdComment!);
    }
}
