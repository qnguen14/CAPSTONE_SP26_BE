using AgroTemp.Domain.DTO.Farm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Service.Interfaces
{
    public interface IFarmService
    {
        Task<List<FarmDTO>> GetFarmByFarmer(Guid farmerProfileId);
        Task<FarmDTO> GetFarmById(Guid id);
        Task<FarmDTO> CreateFarm(Guid farmerProfileId, CreateFarmRequest request);
        Task<FarmDTO> UpdateFarm(Guid id, Guid farmerProfileId, UpdateFarmRequest request);
        Task<bool> DeleteFarm(Guid id, Guid farmerProfileId);

    }
}
