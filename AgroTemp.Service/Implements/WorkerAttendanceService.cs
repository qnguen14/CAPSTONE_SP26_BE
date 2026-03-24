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
    public class WorkerAttendanceService : BaseService<WorkerSession>, IWorkerAttendanceService
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
                var attendance = await _unitOfWork.GetRepository<WorkerSession>()
                    .FirstOrDefaultAsync(
                        predicate: wa => wa.Id == request.AttendanceId,
                        include: q => q.Include(wa => wa.JobDetail)
                                       .ThenInclude(jd => jd.JobPost));

                if (attendance == null)
                {
                    throw new Exception("Attendance record not found");
                }

                // Verify the job post belongs to this farmer
                if (attendance.JobDetail.JobPost.FarmerId != farmerProfileId)
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
                attendance.ApprovedBy = farmerProfileId;
                attendance.ApprovedAt = DateTime.UtcNow;

                if (request.AdjustedHours.HasValue)
                {
                    attendance.TotalHoursWorked = request.AdjustedHours.Value;
                }

                attendance.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<WorkerSession>().UpdateAsync(attendance);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.WorkerSessionToDto(attendance);
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
                    predicate: j => j.Id == request.JobApplicationId && j.WorkerId == workerProfileId,
                    include: q => q.Include(j => j.JobPost));

                if(jobApplication == null)
                {
                    throw new Exception("Job Application not found or does not belong to this worker");
                }

                if(jobApplication.Status != ApplicationStatus.Accepted)
                {
                    throw new Exception("Can only check in for accepted job");
                }

                // Validate check-in time is within job post date range
                if (request.CheckInTime.Date < jobApplication.JobPost.StartDate?.Date)
                {
                    throw new Exception("Cannot check in before job start date");
                }

                if (request.CheckInTime.Date > jobApplication.JobPost.EndDate?.Date)
                {
                    throw new Exception("Cannot check in after job end date");
                }

                // Validate check-in time is not too far in the past (more than 24 hours)
                var hoursSinceCheckIn = (DateTime.UtcNow - request.CheckInTime).TotalHours;
                if (hoursSinceCheckIn > 24)
                {
                    throw new Exception("Cannot check in for a time more than 24 hours in the past");
                }

                // Validate check-in time is not in the future
                if (request.CheckInTime > DateTime.UtcNow)
                {
                    throw new Exception("Cannot check in for a future time");
                }

                var workDate = request.CheckInTime.Date;
                
                // Get or create job detail for this application
                var jobDetail = await _unitOfWork.GetRepository<JobDetail>()
                    .FirstOrDefaultAsync(predicate: jd => 
                        jd.JobApplicationId == request.JobApplicationId &&
                        jd.WorkerId == workerProfileId);

                if (jobDetail == null)
                {
                    // Create job detail if it doesn't exist
                    jobDetail = new JobDetail
                    {
                        Id = Guid.NewGuid(),
                        JobApplicationId = request.JobApplicationId,
                        JobPostId = jobApplication.JobPostId,
                        WorkerId = workerProfileId,
                        StatusId = (int)JobStatus.InProgress,
                        WorkDate = request.CheckInTime.Date,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.GetRepository<JobDetail>().InsertAsync(jobDetail);
                    await _unitOfWork.SaveChangesAsync();
                }

                var existingCheckIn = await _unitOfWork.GetRepository<WorkerSession>()
                    .FirstOrDefaultAsync(predicate: w =>
                    w.JobDetailId == jobDetail.Id &&
                    w.WorkDate == workDate);

                if(existingCheckIn != null)
                {
                    throw new Exception("Already checked in for this job today");
                }

                // Check if worker is currently checked in at another job (hasn't checked out yet)
                var activeCheckIn = await _unitOfWork.GetRepository<WorkerSession>()
                    .FirstOrDefaultAsync(
                        predicate: w => w.JobDetail.WorkerId == workerProfileId &&
                                       w.CheckOutTime == null &&
                                       w.WorkDate == workDate,
                        include: q => q.Include(w => w.JobDetail));

                if (activeCheckIn != null)
                {
                    throw new Exception("You must check out from your current job before checking in to another job");
                }

                var attendance = new WorkerSession
                {
                    Id = Guid.NewGuid(),
                    JobDetailId = jobDetail.Id,
                    WorkDate = workDate,
                    CheckInTime = request.CheckInTime,
                    CheckInNotes = request.CheckInNotes,
                    IsApproved = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<WorkerSession>().InsertAsync(attendance);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.WorkerSessionToDto(attendance);

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
                var attendance = await _unitOfWork.GetRepository<WorkerSession>()
                    .FirstOrDefaultAsync(
                        predicate: wa => wa.Id == request.AttendanceId,
                        include: q => q.Include(wa => wa.JobDetail));

                if (attendance == null)
                {
                    throw new Exception("Attendance record not found");
                }

                if (attendance.JobDetail.WorkerId != workerProfileId)
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

                // Validate check-out time is not in the future
                if (request.CheckOutTime > DateTime.UtcNow)
                {
                    throw new Exception("Cannot check out for a future time");
                }

                // Validate check-out time is not too far in the past (more than 24 hours from check-in)
                var hoursSinceCheckIn = (request.CheckOutTime - attendance.CheckInTime).TotalHours;
                if (hoursSinceCheckIn > 24)
                {
                    throw new Exception("Check-out time cannot be more than 24 hours after check-in time. Please contact support.");
                }

                attendance.CheckOutTime = request.CheckOutTime;
                attendance.CheckOutNotes = request.CheckOutNotes;

                var timeWorked = request.CheckOutTime - attendance.CheckInTime;
                attendance.TotalHoursWorked = (decimal)timeWorked.TotalHours;

                attendance.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<WorkerSession>().UpdateAsync(attendance);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.WorkerSessionToDto(attendance);
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
                var attendance = await _unitOfWork.GetRepository<WorkerSession>()
                    .FirstOrDefaultAsync(predicate: w => w.Id == attendanceId);

                if(attendance == null)
                {
                    throw new Exception("Attendance record not found");
                }

                return _mapper.WorkerSessionToDto(attendance);
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
                var attendanceRecords = await _unitOfWork.GetRepository<WorkerSession>()
                    .GetListAsync(
                        predicate: wa => wa.JobDetail.JobPost.FarmerId == farmerProfileId &&
                                        (!jobPostId.HasValue || wa.JobDetail.JobPostId == jobPostId.Value) &&
                                        (!startDate.HasValue || wa.WorkDate >= startDate.Value.Date) &&
                                        (!endDate.HasValue || wa.WorkDate <= endDate.Value.Date),
                        orderBy: q => q.OrderByDescending(wa => wa.WorkDate).ThenByDescending(wa => wa.CheckInTime),
                        include: q => q.Include(wa => wa.JobDetail)
                                       .ThenInclude(jd => jd.JobPost));

                return _mapper.WorkerSessionsToDtos(attendanceRecords);
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
                var attendanceRecords = await _unitOfWork.GetRepository<WorkerSession>()
                    .GetListAsync(
                        predicate: wa => wa.JobDetail.WorkerId == workerProfileId &&
                                        (!startDate.HasValue || wa.WorkDate >= startDate.Value.Date) &&
                                        (!endDate.HasValue || wa.WorkDate <= endDate.Value.Date),
                        orderBy: q => q.OrderByDescending(wa => wa.WorkDate).ThenByDescending(wa => wa.CheckInTime),
                        include: q => q.Include(wa => wa.JobDetail));

                return _mapper.WorkerSessionsToDtos(attendanceRecords);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //For Farmer
        public async Task<List<WorkerAttendanceDTO>> GetWorkerAttendanceByFarmer(Guid farmerProfileId, Guid workerProfileId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var farmerView = await _unitOfWork.GetRepository<JobApplication>()
            .FirstOrDefaultAsync(
                predicate: ja => ja.JobPost.FarmerId == farmerProfileId &&
                                ja.WorkerId == workerProfileId &&
                                ja.Status == ApplicationStatus.Accepted,
                include: q => q.Include(ja => ja.JobPost));

                if (farmerView == null)
                {
                    throw new Exception("This worker has no accepted applications on your farm");
                }

                var attendanceRecords = await _unitOfWork.GetRepository<WorkerSession>()
                      .GetListAsync(
                          predicate: wa => wa.JobDetail.JobPost.FarmerId == farmerProfileId &&
                                          wa.JobDetail.WorkerId == workerProfileId &&
                                          (!startDate.HasValue || wa.WorkDate >= startDate.Value.Date) &&
                                          (!endDate.HasValue || wa.WorkDate <= endDate.Value.Date),
                          orderBy: q => q.OrderByDescending(wa => wa.WorkDate).ThenByDescending(wa => wa.CheckInTime),
                          include: q => q.Include(wa => wa.JobDetail)
                                         .ThenInclude(jd => jd.JobPost)
                                         .Include(wa => wa.JobDetail)
                                         .ThenInclude(jd => jd.Worker));

                return _mapper.WorkerSessionsToDtos(attendanceRecords);
            }
             catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }           
        }
    }
}

