using Application.Common.Interfaces;
using Application.Features.Likes.Commands;
using Domain.Models;
using MediatR;

namespace Application.Features.Likes.Handlers;

public class PerformLikeCommandHandler(ILikesRepository likesRepository)
    : IRequestHandler<PerformLikeCommand, Like?>
{
    public async Task<Like?> Handle(PerformLikeCommand request, CancellationToken cancellationToken)
    {
        var result = await likesRepository.LikeAsync
        (
            request.UserId,
            request.Entity,
            cancellationToken
        );

        return result;
    }
}