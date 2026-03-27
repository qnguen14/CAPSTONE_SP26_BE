using System.Collections.Generic;

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

