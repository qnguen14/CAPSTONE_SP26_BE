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
        try
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

            if (disputes == null || !disputes.Any())
            {
                return new List<DisputeReportDTO>();
            }

            return _mapper.DisputeReportsToDisputeReportDtos(disputes);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<DisputeReportDTO?> GetDisputeByIdAsync(Guid id)
    {
        try
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

            return _mapper.DisputeReportToDisputeReportDto(dispute);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<DisputeReportDTO> CreateDisputeAsync(CreateDisputeReportRequest request)
    {
        try
        {
            var dispute = _mapper.CreateDisputeReportRequestToDisputeReport(request);
            if (dispute.Id == Guid.Empty)
            {
                dispute.Id = Guid.NewGuid();
            }

            dispute.CreatedAt = DateTime.UtcNow;
            dispute.StatusId = (int)DisputeStatus.Pending;

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

            return _mapper.DisputeReportToDisputeReportDto(created ?? dispute);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<DisputeReportDTO?> UpdateDisputeAsync(Guid id, UpdateDisputeReportRequest request)
    {
        try
        {
            var existing = await _unitOfWork.GetRepository<DisputeReport>()
                .FirstOrDefaultAsync(predicate: d => d.Id == id);

            if (existing == null)
            {
                return null;
            }

            _mapper.UpdateDisputeReportRequestToDisputeReport(request, existing);
            _unitOfWork.GetRepository<DisputeReport>().UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.DisputeReportToDisputeReportDto(existing);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteDisputeAsync(Guid id)
    {
        try
        {
            var existing = await _unitOfWork.GetRepository<DisputeReport>()
                .FirstOrDefaultAsync(predicate: d => d.Id == id);

            if (existing == null)
            {
                return false;
            }

            _unitOfWork.GetRepository<DisputeReport>().DeleteAsync(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
