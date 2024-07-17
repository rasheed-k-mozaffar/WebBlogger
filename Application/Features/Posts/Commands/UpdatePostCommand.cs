using Domain.Enums;
using Domain.Models;
using MediatR;

namespace Application.Features.Posts.Commands;

public record UpdatePostCommand
(
    Guid Id,
    string Title,
    string Content,
    PostStatus Status,
    ICollection<Tag>? Tags
): IRequest<Post?>;