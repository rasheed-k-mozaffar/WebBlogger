using Domain.Enums;
using Domain.Models;
using MediatR;

namespace Application.Features.Posts.Commands;

public class CreatePostCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public PostStatus Status { get; set; } = PostStatus.Published; // defaults to published

    public ICollection<Tag>? Tags { get; set; } = [];
}