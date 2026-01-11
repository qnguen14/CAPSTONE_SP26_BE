using System.Text.Json.Serialization;

namespace AgroTemp.Domain.Metadata;

public class ApiResponse<T>
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }
}