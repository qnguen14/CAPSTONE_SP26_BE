using AgroTemp.Domain.DTO.FarmerProfile;
using System.Threading.Tasks;

namespace AgroTemp.Service.Interfaces
{
    public interface IDashboardService
    {
        Task<FarmerDashboardDTO> GetFarmerDashboardAsync();
    }
}
