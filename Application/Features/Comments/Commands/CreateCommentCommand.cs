using Domain.Models;
using MediatR;

namespace Application.Features.Comments.Commands;

public record CreateCommentCommand
(
    Guid Id,
    Guid PostId,
    Guid? ParentCommentId,
    Guid AuthorId,
    string Content
) : IRequest<Comment>;
