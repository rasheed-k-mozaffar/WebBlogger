namespace Application.Common.Responses;

public abstract class PaginatedCollectionBase(int totalCount, int pageNumber, int pageSize)
{
    public int TotalCount { get; private set; } = totalCount;

    public int PageNumber { get; private set; } = pageNumber;

    public int PageSize { get; private set; } = pageSize;

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;
}