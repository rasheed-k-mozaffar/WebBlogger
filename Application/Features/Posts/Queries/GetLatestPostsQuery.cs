using Domain.Models;
using MediatR;

namespace Application.Features.Posts.Queries;

public class GetLatestPostsQuery(int pageNumber, int pageSize, string? title = null) : IRequest<IReadOnlyCollection<Post>>
{
    public int PageNumber { get; } = pageNumber;

    public int PageSize { get; } = pageSize;

    public string? Title { get; } = title;
}