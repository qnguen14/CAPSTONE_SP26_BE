using System.Text.Json.Serialization;

namespace AgroTemp.Domain.DTO.Notification;

public class ExpoPushPayload
{
    [JsonPropertyName("to")]
    public string To { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("body")]
    public string Body { get; set; }
    
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Data { get; set; }
    
    [JsonPropertyName("sound")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Sound { get; set; } = "default";
    
    [JsonPropertyName("priority")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Priority { get; set; } = "high";
    
    [JsonPropertyName("channelId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ChannelId { get; set; } = "default";
}

public class ExpoPushResponse
{
    [JsonPropertyName("data")]
    public List<ExpoPushTicket> Data { get; set; }
}

public class ExpoPushTicket
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
    [JsonPropertyName("details")]
    public object? Details { get; set; }
}