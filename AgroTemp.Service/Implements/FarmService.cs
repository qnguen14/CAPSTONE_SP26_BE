using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Farm;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Domain.Metadata;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AgroTemp.Service.Implements;

public class FarmService : BaseService<Farm>, IFarmService
{
    private readonly IMapperlyMapper _mapper;

    public FarmService(
        IUnitOfWork<AgroTempDbContext> unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<List<FarmDTO>> GetFarmByFarmer(Guid farmerProfileId)
    {
        try
        {
            var farms = await _unitOfWork.GetRepository<Farm>()
                .GetListAsync(
                    predicate: f => f.FarmerId == farmerProfileId,
                    orderBy: q => q.OrderByDescending(f => f.IsPrimary).ThenBy(f => f.CreatedAt));

            return _mapper.FarmsToDto(farms);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<FarmDTO> GetFarmById(Guid id)
    {
        try
        {
            var farm = await _unitOfWork.GetRepository<Farm>()
                .FirstOrDefaultAsync(predicate: f => f.Id == id);

            if (farm == null)
            {
                throw new Exception("Farm not found");
            }

            return _mapper.FarmToDto(farm);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<FarmDTO> CreateFarm(Guid farmerProfileId, CreateFarmRequest request)
    {
        try
        {
            var farmerProfile = await _unitOfWork.GetRepository<Farmer>()
                .FirstOrDefaultAsync(predicate: fp => fp.Id == farmerProfileId);

            if (farmerProfile == null)
            {
                throw new Exception("Farmer profile not found");
            }

            // Validate farm type specific fields
            if (request.FarmType == FarmType.Livestock && (request.LivestockCount == null || request.LivestockCount <= 0))
                throw new Exception("Livestock count is required and must be greater than 0 for livestock farms.");

            if (request.FarmType == FarmType.Crop && (request.AreaSize == null || request.AreaSize <= 0))
                throw new Exception("Area size is required and must be greater than 0 for crop farms.");

            if (request.isPrimary)
            {
                var existingPrimaryFarms = await _unitOfWork.GetRepository<Farm>()
                    .GetListAsync(predicate: f => f.FarmerId == farmerProfileId && f.IsPrimary);

                foreach (var existingFarm in existingPrimaryFarms)
                {
                    existingFarm.IsPrimary = false;
                    existingFarm.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.GetRepository<Farm>().UpdateAsync(existingFarm);
                }
            }

            var farm = new Farm
            {
                Id = Guid.NewGuid(),
                FarmerId = farmerProfileId,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                LocationName = request.LocationName,
                FarmType = request.FarmType,
                LivestockCount = request.FarmType == FarmType.Livestock ? request.LivestockCount : null,
                AreaSize = request.FarmType == FarmType.Crop ? request.AreaSize : null,
                IsPrimary = request.isPrimary,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Farm>().InsertAsync(farm);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.FarmToDto(farm);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<FarmDTO> UpdateFarm(Guid id, Guid farmerProfileId, UpdateFarmRequest request)
    {
        try
        {
            var farm = await _unitOfWork.GetRepository<Farm>()
                .FirstOrDefaultAsync(predicate: f => f.Id == id);

            if (farm == null)
            {
                throw new Exception("Farm not found");
            }

            if (farm.FarmerId != farmerProfileId)
            {
                throw new Exception("You can only update your own farms");
            }

            if (request.IsPrimary.HasValue && request.IsPrimary.Value && !farm.IsPrimary)
            {
                var existingPrimaryFarms = await _unitOfWork.GetRepository<Farm>()
                    .GetListAsync(predicate: f => f.FarmerId == farmerProfileId && f.IsPrimary && f.Id != id);

                foreach (var existingFarm in existingPrimaryFarms)
                {
                    existingFarm.IsPrimary = false;
                    existingFarm.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.GetRepository<Farm>().UpdateAsync(existingFarm);
                }
            }

            if (!string.IsNullOrEmpty(request.Address))
            {
                farm.Address = request.Address;
            }

            if (request.Latitude.HasValue)
            {
                farm.Latitude = request.Latitude.Value;
            }

            if (request.Longitude.HasValue)
            {
                farm.Longitude = request.Longitude.Value;
            }

            if (!string.IsNullOrEmpty(request.LocationName))
            {
                farm.LocationName = request.LocationName;
            }

            if (request.FarmType.HasValue)
            {
                var newFarmType = request.FarmType.Value;

                if (newFarmType == FarmType.Livestock && (request.LivestockCount == null || request.LivestockCount <= 0))
                    throw new Exception("Livestock count is required and must be greater than 0 for livestock farms.");

                if (newFarmType == FarmType.Crop && (request.AreaSize == null || request.AreaSize <= 0))
                    throw new Exception("Area size is required and must be greater than 0 for crop farms.");

                farm.FarmType = newFarmType;
                farm.LivestockCount = newFarmType == FarmType.Livestock ? request.LivestockCount : null;
                farm.AreaSize = newFarmType == FarmType.Crop ? request.AreaSize : null;
            }
            else
            {
                // FarmType unchanged — update individual fields if provided
                if (farm.FarmType == FarmType.Livestock && request.LivestockCount.HasValue)
                {
                    if (request.LivestockCount <= 0)
                        throw new Exception("Livestock count must be greater than 0.");
                    farm.LivestockCount = request.LivestockCount.Value;
                }

                if (farm.FarmType == FarmType.Crop && request.AreaSize.HasValue)
                {
                    if (request.AreaSize <= 0)
                        throw new Exception("Area size must be greater than 0.");
                    farm.AreaSize = request.AreaSize.Value;
                }
            }

            if (request.IsPrimary.HasValue)
            {
                farm.IsPrimary = request.IsPrimary.Value;
            }

            farm.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Farm>().UpdateAsync(farm);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.FarmToDto(farm);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteFarm(Guid id, Guid farmerProfileId)
    {
        try
        {
            var farm = await _unitOfWork.GetRepository<Farm>()
                .FirstOrDefaultAsync(predicate: f => f.Id == id);

            if (farm == null)
            {
                throw new Exception("Farm not found");
            }

            if (farm.FarmerId != farmerProfileId)
            {
                throw new Exception("You can only delete your own farms");
            }

            _unitOfWork.GetRepository<Farm>().DeleteAsync(farm);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}