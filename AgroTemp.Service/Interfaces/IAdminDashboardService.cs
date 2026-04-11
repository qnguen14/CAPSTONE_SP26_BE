using AgroTemp.Domain.DTO.Admin;

namespace AgroTemp.Service.Interfaces;

public interface IAdminDashboardService
{
    Task<AdminDashboardResponse> GetDashboardAsync(int trendMonths = 6, int activityLimit = 20);
}
