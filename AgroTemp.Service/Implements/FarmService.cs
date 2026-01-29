using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO;
using AgroTemp.Domain.DTO.Farm;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
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
                    predicate: f => f.FarmerProfileId == farmerProfileId,
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
            var farmerProfile = await _unitOfWork.GetRepository<FarmerProfile>()
                .FirstOrDefaultAsync(predicate: fp => fp.Id == farmerProfileId);

            if (farmerProfile == null)
            {
                throw new Exception("Farmer profile not found");
            }

            if (request.isPrimary)
            {
                var existingPrimaryFarms = await _unitOfWork.GetRepository<Farm>()
                    .GetListAsync(predicate: f => f.FarmerProfileId == farmerProfileId && f.IsPrimary);

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
                FarmerProfileId = farmerProfileId,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                LocationName = request.LocationName,
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

            if (farm.FarmerProfileId != farmerProfileId)
            {
                throw new Exception("You can only update your own farms");
            }

            if (request.IsPrimary.HasValue && request.IsPrimary.Value && !farm.IsPrimary)
            {
                var existingPrimaryFarms = await _unitOfWork.GetRepository<Farm>()
                    .GetListAsync(predicate: f => f.FarmerProfileId == farmerProfileId && f.IsPrimary && f.Id != id);

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

            if (farm.FarmerProfileId != farmerProfileId)
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