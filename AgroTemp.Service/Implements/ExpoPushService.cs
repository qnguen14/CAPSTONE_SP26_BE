using System.Net.Http.Json;
using System.Text.Json;
using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Notification;
using AgroTemp.Domain.Entities;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace AgroTemp.Service.Implements;

public class ExpoPushService : IExpoPushService
{
    private readonly IUnitOfWork<AgroTempDbContext> _unitOfWork;
    private readonly ILogger<ExpoPushService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string EXPO_PUSH_URL = "https://exp.host/--/api/v2/push/send";

    public ExpoPushService(
        IUnitOfWork<AgroTempDbContext> unitOfWork,
        ILogger<ExpoPushService> logger,
        IHttpClientFactory httpClientFactory
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> SendPushNotificationAsync(Guid userId, string title, string body, Dictionary<string, object>? data = null)
    {
        try
        {
            var tokens = await _unitOfWork.GetRepository<DeviceToken>()
                .GetListAsync(
                    predicate: dt => dt.UserId == userId && dt.IsActive,
                    include: null);

            if(tokens == null || !tokens.Any())
            {
                _logger.LogWarning("No active device tokens found for user {userId}", userId);
                return false;
            }        

            var tokenStrings = tokens.Select(t => t.ExpoPushToken).ToList();
            var results = await SendPushToMultipleAsync(tokenStrings, title, body, data);

            return results.Any();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification to user {userId}", userId);
            return false;
        }
    }

    public async Task<bool> SendPushToTokenAsync(string expoPushToken, string title, string body, Dictionary<string, object>? data = null)
    {
        var results = await SendPushToMultipleAsync(new List<string> { expoPushToken }, title, body, data);
        return results.Any();
    }

    public async Task<List<string>> SendPushToMultipleAsync(List<string> tokens, string title, string body, Dictionary<string, object>? data = null)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var successfulTokens = new List<string>();

            var payloads = tokens.Select(token => new ExpoPushPayload
            {
                To = token,
                Title = title,
                Body = body,
                Data = data,
                Sound = "default",
                Priority = "high"
            }).ToList();

            var response = await httpClient.PostAsJsonAsync(EXPO_PUSH_URL, payloads);
            
            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ExpoPushResponse>();

                if(result?.Data != null)
                {
                    for(int i = 0; i < result.Data.Count; i++)
                    {
                        var ticket = result.Data[i];
                        if(ticket.Status == "ok")
                        {
                            successfulTokens.Add(tokens[i]);
                            _logger.LogInformation("Push notification sent successfully to token {token}", tokens[i]);
                        }
                        else
                        {
                            _logger.LogWarning($"Push failed for token {tokens[i]}: {ticket.Message}");                        
                        }
                    }
                }
            }
            else
            {
                _logger.LogError($"Expo API returned error: {response.StatusCode}");            
            }

            return successfulTokens;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notifications to Expo");
            return new List<string>();
        }
    }
}
