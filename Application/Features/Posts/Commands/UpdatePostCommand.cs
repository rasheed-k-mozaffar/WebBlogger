using Domain.Enums;
using Domain.Models;
using MediatR;

namespace Application.Features.Posts.Commands;

public class UpdatePostCommand : IRequest<Post?>
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public PostStatus Status { get; set; }

    public ICollection<Tag>? Tags { get; set; } = [];
}