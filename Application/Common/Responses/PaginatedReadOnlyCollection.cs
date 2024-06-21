using System.Collections.ObjectModel;
using Domain.Models;

namespace Application.Common.Responses;

public class PaginatedReadOnlyCollection<T>(int totalCount, int pageNumber, int pageSize)
    : PaginatedCollectionBase(totalCount, pageNumber, pageSize)
{
    public ReadOnlyCollection<T> Items { get; }

    public PaginatedReadOnlyCollection
        (int totalCount, int pageNumber, int pageSize, ReadOnlyCollection<T> items)
        : this(totalCount, pageNumber, pageSize)
    {
        Items = items;
    }

    public static PaginatedReadOnlyCollection<T> Empty(int pageNumber, int pageSize)
        => new PaginatedReadOnlyCollection<T>(0, pageNumber, pageSize, new List<T>().AsReadOnly());
}