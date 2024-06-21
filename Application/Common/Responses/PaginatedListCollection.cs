namespace Application.Common.Responses;

public class PaginatedListCollection<T>(int totalCount, int pageNumber, int pageSize)
    : PaginatedCollectionBase(totalCount, pageNumber, pageSize)
    where T : class, new()
{
    public List<T> Items { get; } = [];

    public PaginatedListCollection
        (int totalCount, int pageNumber, int pageSize, List<T> items)
        : this(totalCount, pageNumber, pageSize)
    {
        Items = items;
    }

    public static PaginatedListCollection<T> Empty(int pageNumber, int pageSize)
        => new PaginatedListCollection<T>(0, pageNumber, pageSize, new List<T>());
}