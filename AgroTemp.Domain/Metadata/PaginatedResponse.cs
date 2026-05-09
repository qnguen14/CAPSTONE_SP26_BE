using AgroTemp.Domain.DTO;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgroTemp.Domain.Metadata;

public class PaginationMetadata
{
    public int Page { get; set; }

    public int Limit { get; set; }

    public int Total { get; set; }

    public int TotalPages { get; set; }
}

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();

    public PaginationMetadata Pagination { get; set; } = new();
}

public class PaginatedDisputeResponse<T>
{
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new();

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("page_number")]
    public int PageNumber { get; set; }

    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    [JsonPropertyName("workers")]
    public List<WorkerProfileDTO>? Workers { get; set; }

    [JsonPropertyName("farmers")]
    public List<FarmerProfileDTO>? Farmers { get; set; }
}

