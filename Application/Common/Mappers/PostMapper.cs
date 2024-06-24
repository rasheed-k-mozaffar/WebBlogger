using Application.Features.Posts.Commands;
using Domain.Models;

namespace Application.Common.Mappers;

public static class PostMapper
{
    public static Post MapToPost(this CreatePostCommand command) =>
        new Post()
        {
            Id = command.Id,
            Title = command.Title,
            Content = command.Content,
            Tags = command.Tags,
            PublishedOn = DateTime.UtcNow
        };

    public static Post MapToPost(this UpdatePostCommand command) =>
        new Post()
        {
            Id = command.Id,
            Title = command.Title,
            Content = command.Content,
            Tags = command.Tags,
            Status = command.Status
        };
}