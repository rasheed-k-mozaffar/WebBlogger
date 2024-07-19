using MediatR;

namespace Application.Features.Comments.Commands;

public record DeleteCommentCommand
(
    Guid Id
): IRequest<bool>;