using Application.Common.Interfaces;
using Application.Common.Responses;
using Application.Features.Posts.Queries;
using Domain.Models;
using MediatR;

namespace Application.Features.Posts.Handlers;

public class GetLatestPostsQueryHandler(IPostsRepository postsRepository)
    : IRequestHandler<GetLatestPostsQuery, PaginatedReadOnlyCollection<Post>>
{
    public async Task<PaginatedReadOnlyCollection<Post>> Handle(GetLatestPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await postsRepository
            .GetLatestPostsAsync(request.PageNumber, request.PageSize, request.Title, request.SortOption,
                cancellationToken);

        return posts;
    }
}