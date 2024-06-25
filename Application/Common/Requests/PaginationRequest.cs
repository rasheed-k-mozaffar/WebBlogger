namespace Application.Common.Requests;

public class PaginationRequest<T>(int pageNumber, int pageSize, string? searchTerm = null) where T: Enum
{
    public int PageNumber { get; } = pageNumber;

    public int PageSize { get; } = pageSize;

    public string? SearchTerm { get; } = searchTerm;

    public T? SortOption { get; set; }

    public PaginationRequest(int pageNumber, int pageSize, T? sortOption, string? searchTerm = null)
        : this(pageNumber, pageSize, searchTerm)
    {
        SortOption = sortOption;
    }
}