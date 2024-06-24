using Application.Common.Interfaces;
using Application.Features.Posts.Commands;
using MediatR;

namespace Application.Features.Posts.Handlers;

public class DeletePostCommandHandler(IPostsRepository postsRepository) : IRequestHandler<DeletePostCommand>
{
    public async Task Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        await postsRepository.DeletePostAsync(request.PostId, request.DeleteType, cancellationToken);
    }
}