using Application.Features.Comments.Commands;
using Domain.Models;

namespace Application.Common.Mappers;

public static class CommentMapper
{
    public static Comment ToComment(this CreateCommentCommand command) =>
        new Comment()
        {
            Id = command.Id,
            PostId = command.PostId,
            ParentCommentId = command.ParentCommentId,
            AuthorId = command.AuthorId,
            Content = command.Content,
        };

    public static Comment ToComment(this UpdateCommentCommand command) =>
        new Comment()
        {
            Id = command.Id,
            Content = command.Content,
        };
}