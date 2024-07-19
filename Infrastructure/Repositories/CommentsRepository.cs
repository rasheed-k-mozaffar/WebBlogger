using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Requests;
using Application.Common.Responses;
using Application.Extensions;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CommentsRepository(AppDbContext dbContext) : ICommentsRepository
{
    public async Task<PaginatedListCollection<Comment>> GetCommentsAsync(Guid postId, PaginationRequest<CommentsSortOption> paginationRequest, CancellationToken cancellationToken)
    {
        var query = dbContext
            .Comments
            .Include(p => p.Replies)
            .Where(c => c.PostId == postId);

        var totalCount = await query.CountAsync(cancellationToken);

        var paginatedComments = await query
            .ApplyFilter(paginationRequest.SearchTerm)
            .ApplySorting(paginationRequest.SortOption)
            .ApplyPagination(paginationRequest.PageNumber, paginationRequest.PageSize)
            .Execute(cancellationToken);

        return new PaginatedListCollection<Comment>
        (
            pageNumber: paginationRequest.PageNumber,
            pageSize: paginationRequest.PageSize,
            totalCount: totalCount,
            items: paginatedComments
        );
    }

    public async Task<Comment> SaveCommentAsync(Guid postId, Comment comment, CancellationToken cancellationToken)
    {
        var post = await dbContext
            .Posts
            .FindAsync(postId, cancellationToken);

        if (post is null)
            throw new PostNotFoundException("The post you're looking for was not found");

        if (comment.ParentCommentId.HasValue)
        {
            // grab the parent comment to set the added comment as a reply to the parent
            var parentComment = await GetParentCommentAndThrowIfNullAsync
                (comment.ParentCommentId.Value, cancellationToken);
            comment.ParentComment = parentComment;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return comment;
    }

    public async Task<Comment> GetCommentByIdAsync(Guid commentId, CancellationToken cancellationToken)
    {
        var comment = await dbContext
            .Comments
            .Include(p => p.Replies)
            .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);

        if(comment is null)
            throw new CommentNotFoundException("The comment you're looking for was not found");

        return comment;
    }

    public async Task<Comment> UpdateCommentAsync(Guid commentId, Comment comment, CancellationToken cancellationToken)
    {
        var commentToUpdate = await FindCommentAndThrowIfNull(commentId, cancellationToken);

        commentToUpdate.Content = comment.Content;
        commentToUpdate.LastEditedOn = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return commentToUpdate;
    }

    public async Task<bool> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken)
    {
        var commentToDelete = await FindCommentAndThrowIfNull
        (
            commentId,
            cancellationToken,
            "The comment you're trying to delete for was not found"
        );

        dbContext.Remove(commentToDelete);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    #region Helper methods
    private async Task<Comment> GetParentCommentAndThrowIfNullAsync(Guid parentCommentId,
        CancellationToken cancellationToken)
    {
        var parentComment = await dbContext
            .Comments
            .FirstOrDefaultAsync(c => c.Id == parentCommentId, cancellationToken);

        if (parentComment is null)
            throw new CommentNotFoundException("The comment you're replying too was not found");

        return parentComment;
    }

    private async Task<Comment> FindCommentAndThrowIfNull
        (Guid commentId, CancellationToken cancellationToken, string? exceptionMessage = null)
    {
        var comment = await dbContext
            .Comments
            .FindAsync(commentId);

        if(comment is null)
            throw new CommentNotFoundException(exceptionMessage ?? "The comment you're looking for was not found");

        return comment;
    }
    #endregion
}