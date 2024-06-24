using Application.Common.Enums;
using Application.Common.Responses;
using Domain.Models;
using MediatR;

namespace Application.Features.Posts.Queries;

public class GetLatestPostsQuery(int pageNumber, int pageSize, PostSortOption sortOption, string? title = null) : IRequest<PaginatedReadOnlyCollection<Post>>
{
    public int PageNumber { get; } = pageNumber;

    public int PageSize { get; } = pageSize;

    public string? Title { get; } = title;

    public PostSortOption SortOption { get; } = sortOption;
}