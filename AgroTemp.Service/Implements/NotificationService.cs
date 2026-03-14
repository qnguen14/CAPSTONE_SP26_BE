using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Notification;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AgroTemp.Service.Implements;

public class NotificationService : BaseService<Notification>, INotificationService
{
    private readonly IMapperlyMapper _mapper;

    public NotificationService(
        IUnitOfWork<AgroTempDbContext> unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
    {
        _mapper = mapper;
    }

    public async Task<NotificationDTO> CreateAsync(CreateNotificationRequest request)
    {
        try
        {
            var entity = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = request.Type,
                Title = request.Title,
                Message = request.Message,
                RelatedEntityId = request.RelatedEntityId,
                IsRead = false,
                SentAt = DateTime.UtcNow,
                ReadAt = null
            };

            await _unitOfWork.GetRepository<Notification>().InsertAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.NotificationToDto(entity);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<NotificationDTO>> GetByUserAsync(Guid userId)
    {
        try
        {
            var notifications = await _unitOfWork.GetRepository<Notification>()
                .GetListAsync(
                    predicate: n => n.UserId == userId,
                    orderBy: n => n.OrderByDescending(x => x.SentAt));

            if (notifications == null || !notifications.Any())
                return new List<NotificationDTO>();

            return _mapper.NotificationsToDto(notifications);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<NotificationDTO>> GetUnreadByUserAsync(Guid userId)
    {
        try
        {
            var notifications = await _unitOfWork.GetRepository<Notification>()
                .GetListAsync(
                    predicate: n => n.UserId == userId && !n.IsRead,
                    orderBy: n => n.OrderByDescending(x => x.SentAt));

            if (notifications == null || !notifications.Any())
                return new List<NotificationDTO>();

            return _mapper.NotificationsToDto(notifications);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        try
        {
            var notification = await _unitOfWork.GetRepository<Notification>()
                .FirstOrDefaultAsync(predicate: n => n.Id == notificationId);

            if (notification == null)
                throw new Exception("Notification not found");

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Notification>().UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        try
        {
            var notifications = await _unitOfWork.GetRepository<Notification>()
                .GetListAsync(predicate: n => n.UserId == userId && !n.IsRead);

            if (notifications == null || !notifications.Any())
                return;

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<Notification>().UpdateAsync(notification);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteAsync(Guid notificationId)
    {
        try
        {
            var notification = await _unitOfWork.GetRepository<Notification>()
                .FirstOrDefaultAsync(predicate: n => n.Id == notificationId);

            if (notification == null)
                throw new Exception("Notification not found");

            _unitOfWork.GetRepository<Notification>().DeleteAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
