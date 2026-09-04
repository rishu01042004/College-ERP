using System.ComponentModel.DataAnnotations;

namespace CollegeERP.Api.DTOs;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed record ApiMessage(string Message);

public class QueryParameters
{
    public string? Search { get; set; }
    [Range(1, int.MaxValue)] public int Page { get; set; } = 1;
    [Range(1, 100)] public int PageSize { get; set; } = 20;
}

public sealed class EntityQueryParameters
{
    public QueryParameters Base { get; set; } = new QueryParameters();
    public string? Status { get; set; }
    public string? Department { get; set; }

    public string? Search { get => Base.Search; set => Base.Search = value; }
    public int Page { get => Base.Page; set => Base.Page = value; }
    public int PageSize { get => Base.PageSize; set => Base.PageSize = value; }
}

