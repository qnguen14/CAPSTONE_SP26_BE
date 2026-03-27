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
    private readonly IExpoPushService _expoPushService;

    public NotificationService(
        IUnitOfWork<AgroTempDbContext> unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IMapperlyMapper mapper,
        IExpoPushService expoPushService) : base(unitOfWork, httpContextAccessor, mapper)
    {
        _mapper = mapper;
        _expoPushService = expoPushService;
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

            await _expoPushService.SendPushNotificationAsync(
                request.UserId,
                request.Title,
                request.Message,
                request.RelatedEntityId.HasValue
                    ? new Dictionary<string, object> { { "relatedEntityId", request.RelatedEntityId.Value.ToString() }, { "type", request.Type.ToString() } }
                    : new Dictionary<string, object> { { "type", request.Type.ToString() } }
            );

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

    public async Task<NotificationDTO> MarkAsReadAsync(Guid notificationId)
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

            return _mapper.NotificationToDto(notification);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<NotificationDTO>> MarkAllAsReadAsync(Guid userId)
    {
        try
        {
            var notifications = await _unitOfWork.GetRepository<Notification>()
                .GetListAsync(predicate: n => n.UserId == userId && !n.IsRead);

            if (notifications == null || !notifications.Any())
                return new List<NotificationDTO>();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<Notification>().UpdateAsync(notification);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.NotificationsToDto(notifications);
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

    public async Task RegisterDeviceTokenAsync(Guid userId, string token, string? deviceName = null)
    {
        try
        {
            var existing = await _unitOfWork.GetRepository<DeviceToken>()
                .FirstOrDefaultAsync(
                    predicate: dt => dt.UserId == userId && dt.ExpoPushToken == token);

            if(existing != null)
            {
                existing.IsActive = true;
                existing.LastUsedAt = DateTime.UtcNow;
                if(deviceName != null)
                {
                    existing.DeviceName = deviceName;
                }

                _unitOfWork.GetRepository<DeviceToken>().UpdateAsync(existing);
            }
            else
            {
                var newToken = new DeviceToken
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ExpoPushToken = token,
                    Platform = DevicePlatform.Android,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LastUsedAt = DateTime.UtcNow,
                };

                await _unitOfWork.GetRepository<DeviceToken>().InsertAsync(newToken);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error registering device token: {ex.Message}");
        }
    }

    public async Task UnregisterDeviceTokenAsync(Guid userId, string token)
    {
        try
        {
            var deviceToken = await _unitOfWork
            .GetRepository<DeviceToken>()
            .FirstOrDefaultAsync(
                predicate: dt => dt.UserId == userId && dt.ExpoPushToken == token
            );

            if(deviceToken != null)
            {
                deviceToken.IsActive = false;
                _unitOfWork.GetRepository<DeviceToken>().UpdateAsync(deviceToken);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error unregistering device token: {ex.Message}");
        }
    }

    public async Task<List<string>> GetActiveTokensByUserAsync(Guid userId)
    {
        try
        {
            var tokens = await _unitOfWork.GetRepository<DeviceToken>()
            .GetListAsync(
                predicate: dt => dt.UserId == userId && dt.IsActive);

            return tokens?.Select(t => t.ExpoPushToken).ToList() ?? new List<string>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting active tokens: {ex.Message}");
        }
    }

    public async Task<bool> SendPushNotificationAsync(Guid userId, string title, string body, Dictionary<string, object>? data = null)
    {
        try
        {
            return await _expoPushService.SendPushNotificationAsync(userId, title, body, data);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error sending push notification: {ex.Message}");
        }
    }
}
