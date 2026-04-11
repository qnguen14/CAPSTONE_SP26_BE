using AgroTemp.Domain.DTO.FarmerProfile;
using AgroTemp.Domain.DTO.WorkerProfile;
using System.Threading.Tasks;

namespace AgroTemp.Service.Interfaces
{
    public interface IDashboardService
    {
        Task<FarmerDashboardDTO> GetFarmerDashboardAsync();
        Task<WorkerApplicationStatsDTO> GetWorkerDashboardAsync();
    }
}
