using AgroTemp.Domain.DTO.WorkerAttendance;
using AgroTemp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroTemp.Service.Interfaces
{
    public interface IWorkerAttendanceService
    {
        Task<WorkerAttendanceDTO> CheckIn(Guid workerProfileId, CheckInRequest request);
        Task<WorkerAttendanceDTO> CheckOut(Guid workerProfileId, CheckOutRequest request);
        Task<WorkerAttendanceDTO> ApproveAttendance(Guid farmerProfileId, ApproveAttendanceRequest request);
        Task<WorkerAttendanceDTO> GetAttendanceById(Guid attendanceId);
        Task<List<WorkerAttendanceDTO>> GetWorkerAttendanceHistory(Guid workerProfileId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<WorkerAttendanceDTO>> GetFarmAttendanceRecords(Guid farmerProfileId, Guid? jobPostId = null, DateTime? startDate = null, DateTime? endDate = null);

    }
}
