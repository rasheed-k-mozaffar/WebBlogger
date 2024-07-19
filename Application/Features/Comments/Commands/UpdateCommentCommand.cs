using Domain.Models;
using MediatR;

namespace Application.Features.Comments.Commands;

public record UpdateCommentCommand
(
    Guid Id,
    string Content
) : IRequest<Comment>;