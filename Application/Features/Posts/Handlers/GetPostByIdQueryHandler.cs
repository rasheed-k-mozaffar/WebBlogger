using Application.Common.Interfaces;
using Application.Features.Posts.Queries;
using Domain.Models;
using MediatR;

namespace Application.Features.Posts.Handlers;

public class GetPostByIdQueryHandler(IPostsRepository postsRepository)
    : IRequestHandler<GetPostByIdQuery, Post?>
{
    public async Task<Post?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await postsRepository
            .GetPostByIdAsync(request.PostId, cancellationToken);

        return post;
    }
}