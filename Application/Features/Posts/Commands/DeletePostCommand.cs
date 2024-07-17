using Domain.Enums;
using MediatR;

namespace Application.Features.Posts.Commands;

public record DeletePostCommand
(
    Guid PostId,
    DeleteType DeleteType = DeleteType.Soft
) : IRequest;