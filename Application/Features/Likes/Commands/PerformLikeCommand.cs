using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Likes.Commands;

public record PerformLikeCommand
(
    Guid UserId,
    ILikeable Entity
): IRequest<Like?>;