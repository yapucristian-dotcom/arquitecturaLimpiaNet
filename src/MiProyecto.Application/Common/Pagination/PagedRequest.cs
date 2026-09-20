namespace MiProyecto.Application.Common.Pagination;

public class PagedRequest
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }
}
