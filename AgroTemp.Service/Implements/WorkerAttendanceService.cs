using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.WorkerAttendance;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgroTemp.Service.Implements
{
    public class WorkerAttendanceService : BaseService<WorkerAttendance>, IWorkerAttendanceService
    {
        private readonly IMapperlyMapper _mapper;

        public WorkerAttendanceService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }


        public async Task<WorkerAttendanceDTO> ApproveAttendance(Guid farmerProfileId, ApproveAttendanceRequest request)
        {
            try
            {
                // Get attendance record with related data
                var attendance = await _unitOfWork.GetRepository<WorkerAttendance>()
                    .FirstOrDefaultAsync(
                        predicate: wa => wa.Id == request.AttendanceId,
                        include: q => q.Include(wa => wa.JobApplication)
                                       .ThenInclude(ja => ja.JobPost));

                if (attendance == null)
                {
                    throw new Exception("Attendance record not found");
                }

                // Verify the job post belongs to this farmer
                if (attendance.JobApplication.JobPost.FarmerProfileId != farmerProfileId)
                {
                    throw new Exception("You can only approve attendance for your own job posts");
                }

                // Check if worker has checked out
                if (!attendance.CheckOutTime.HasValue)
                {
                    throw new Exception("Cannot approve attendance before worker has checked out");
                }

                // Check if already approved
                if (attendance.IsApproved)
                {
                    throw new Exception("Attendance already approved");
                }

                // Approve and optionally adjust hours
                attendance.IsApproved = true;
                attendance.ApprovedBy = request.ApprovedBy;
                attendance.ApprovedAt = DateTime.UtcNow;

                if (request.AdjustedHours.HasValue)
                {
                    attendance.TotalHoursWorked = request.AdjustedHours.Value;
                }

                attendance.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<WorkerAttendance>().UpdateAsync(attendance);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.WorkerAttendanceToDto(attendance);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WorkerAttendanceDTO> CheckIn(Guid workerProfileId, CheckInRequest request)
        {
            try
            {
                var jobApplication = await _unitOfWork.GetRepository<JobApplication>()
                    .FirstOrDefaultAsync(
                    predicate: j => j.Id == request.JobApplicationId && j.WorkerProfileId == workerProfileId,
                    include: q => q.Include(j => j.JobPost));

                if(jobApplication == null)
                {
                    throw new Exception("Job Application not found or does not belong to this worker");
                }

                if(jobApplication.Status != ApplicationStatus.Accepted)
                {
                    throw new Exception("Can only check in for accepted job");
                }

                var workDate = request.CheckInTime.Date;
                var existingCheckIn = await _unitOfWork.GetRepository<WorkerAttendance>()
                    .FirstOrDefaultAsync(predicate: w =>
                    w.JobApplicationId == request.JobApplicationId &&
                    w.WorkDate == workDate);

                if(existingCheckIn != null)
                {
                    throw new Exception("Already check in for job accepted");
                }

                var attendance = new WorkerAttendance
                {
                    Id = Guid.NewGuid(),
                    JobApplicationId = request.JobApplicationId,
                    WorkDate = workDate,
                    CheckInTime = request.CheckInTime,
                    CheckInNotes = request.CheckInNotes,
                    IsApproved = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<WorkerAttendance>().InsertAsync(attendance);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.WorkerAttendanceToDto(attendance);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WorkerAttendanceDTO> CheckOut(Guid workerProfileId, CheckOutRequest request)
        {
            try
            {
                var attendance = await _unitOfWork.GetRepository<WorkerAttendance>()
                    .FirstOrDefaultAsync(
                        predicate: wa => wa.Id == request.AttendanceId,
                        include: q => q.Include(wa => wa.JobApplication));

                if (attendance == null)
                {
                    throw new Exception("Attendance record not found");
                }

                if (attendance.JobApplication.WorkerProfileId != workerProfileId)
                {
                    throw new Exception("This attendance record does not belong to you");
                }

                if (attendance.CheckOutTime.HasValue)
                {
                    throw new Exception("Already checked out");
                }

                if (request.CheckOutTime <= attendance.CheckInTime)
                {
                    throw new Exception("Check-out time must be after check-in time");
                }

                attendance.CheckOutTime = request.CheckOutTime;
                attendance.CheckOutNotes = request.CheckOutNotes;

                var timeWorked = request.CheckOutTime - attendance.CheckInTime;
                attendance.TotalHoursWorked = (decimal)timeWorked.TotalHours;

                attendance.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<WorkerAttendance>().UpdateAsync(attendance);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.WorkerAttendanceToDto(attendance);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<WorkerAttendanceDTO> GetAttendanceById(Guid attendanceId)
        {
            try
            {
                var attendance = await _unitOfWork.GetRepository<WorkerAttendance>()
                    .FirstOrDefaultAsync(predicate: w => w.Id == attendanceId);

                if(attendance == null)
                {
                    throw new Exception("Attendance record not found");
                }

                return _mapper.WorkerAttendanceToDto(attendance);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<WorkerAttendanceDTO>> GetFarmAttendanceRecords(Guid farmerProfileId, Guid? jobPostId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var attendanceRecords = await _unitOfWork.GetRepository<WorkerAttendance>()
                    .GetListAsync(
                        predicate: wa => wa.JobApplication.JobPost.FarmerProfileId == farmerProfileId &&
                                        (!jobPostId.HasValue || wa.JobApplication.JobPostId == jobPostId.Value) &&
                                        (!startDate.HasValue || wa.WorkDate >= startDate.Value.Date) &&
                                        (!endDate.HasValue || wa.WorkDate <= endDate.Value.Date),
                        orderBy: q => q.OrderByDescending(wa => wa.WorkDate).ThenByDescending(wa => wa.CheckInTime),
                        include: q => q.Include(wa => wa.JobApplication)
                                       .ThenInclude(ja => ja.JobPost));

                return _mapper.WorkerAttendancesToDtos(attendanceRecords);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<WorkerAttendanceDTO>> GetWorkerAttendanceHistory(Guid workerProfileId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var attendanceRecords = await _unitOfWork.GetRepository<WorkerAttendance>()
                    .GetListAsync(
                        predicate: wa => wa.JobApplication.WorkerProfileId == workerProfileId &&
                                        (!startDate.HasValue || wa.WorkDate >= startDate.Value.Date) &&
                                        (!endDate.HasValue || wa.WorkDate <= endDate.Value.Date),
                        orderBy: q => q.OrderByDescending(wa => wa.WorkDate).ThenByDescending(wa => wa.CheckInTime),
                        include: q => q.Include(wa => wa.JobApplication));

                return _mapper.WorkerAttendancesToDtos(attendanceRecords);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
