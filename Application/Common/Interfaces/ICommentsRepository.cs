using Application.Common.Enums;
using Application.Common.Requests;
using Application.Common.Responses;
using Domain.Models;

namespace Application.Common.Interfaces;

public interface ICommentsRepository
{
    Task<PaginatedListCollection<Comment>> GetCommentsAsync
        (Guid postId, PaginationRequest<CommentsSortOption> sortOption, CancellationToken cancellationToken);

    Task<Comment> SaveCommentAsync
        (Guid postId, Comment comment, Guid? parentCommentId, CancellationToken cancellationToken);

    Task<Comment> GetCommentByIdAsync(Guid commentId, CancellationToken cancellationToken);

    Task<Comment> UpdateCommentAsync(Guid commentId, Comment comment, CancellationToken cancellationToken);

    Task<bool> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken);
}