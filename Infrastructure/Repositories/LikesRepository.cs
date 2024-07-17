using Application.Common.Interfaces;
using Application.Common.Requests;
using Application.Common.Responses;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LikesRepository(AppDbContext dbContext) : ILikesRepository
{
    public async Task<bool> IsLikedAsync(Guid userId, ILikeable entity, CancellationToken cancellationToken)
    {
        var type = entity.GetType();

        if (type == typeof(Post) && entity is Post post)
        {
            var checkedPost = await dbContext
                .Posts
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == post.Id, cancellationToken);

            if (checkedPost is null)
                return false;

            return checkedPost.Likes.Any(x => x.AppUserId == userId);
        }

        if (type == typeof(Comment) && entity is Comment comment)
        {
            var checkedComment = await dbContext
                .Comments
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == comment.Id, cancellationToken);

            if (checkedComment is null)
                return false;

            return checkedComment.Likes.Any(x => x.AppUserId == userId);
        }

        return false;
    }

    public async Task<Like?> LikeAsync(Guid userId, ILikeable entity, CancellationToken cancellationToken)
    {
        return entity switch
        {
            Post post => await HandleLikingAsync(userId, post, cancellationToken),
            Comment comment => await HandleLikingAsync(userId, comment, cancellationToken),
            _ => null
        };
    }

    public async Task<PaginatedReadOnlyCollection<Like>> GetLikesAsync(ILikeable entity, PaginationRequest paginationRequest, CancellationToken cancellationToken)
    {
        if (entity is Post post)
        {
            var query = dbContext
                .Likes
                .Where(like => like.PostId == post.Id);

            var totalCount = await query.CountAsync(cancellationToken);

            var paginatedLikes = await GetPaginatedLikesAsync
                (query, paginationRequest, cancellationToken);

            return new PaginatedReadOnlyCollection<Like>
            (
                totalCount: totalCount,
                pageNumber: paginationRequest.PageNumber,
                pageSize: paginationRequest.PageSize,
                items: paginatedLikes.AsReadOnly()
            );
        }
        else if(entity is Comment comment)
        {
            var query = dbContext
                .Likes
                .Where(like => like.CommentId == comment.Id);

            var totalCount = await query.CountAsync(cancellationToken);

            var paginatedLikes = await GetPaginatedLikesAsync
                (query, paginationRequest, cancellationToken);

            return new PaginatedReadOnlyCollection<Like>
            (
                totalCount: totalCount,
                pageNumber: paginationRequest.PageNumber,
                pageSize: paginationRequest.PageSize,
                items: paginatedLikes.AsReadOnly()
            );
        }
        else
        {
            return PaginatedReadOnlyCollection<Like>.Empty
            (
                paginationRequest.PageNumber,
                paginationRequest.PageSize
            );
        }
    }

    #region Helpers
    private async Task<Like?> HandleLikingAsync<T>(Guid userId, T entity, CancellationToken cancellationToken)
        where T: class, ILikeable
    {
        var existingLike = await dbContext
                .Likes
                .FirstOrDefaultAsync(like => like.AppUserId == userId, cancellationToken);

        var type = entity.GetType();

        if (existingLike is not null)
        {
            // if the user has already liked the entity, then remove the like
            // and decrement the likes count by one
            dbContext.Likes.Remove(existingLike);
            entity.LikeCount--;
            await dbContext.SaveChangesAsync(cancellationToken);
            return null;
        }

        if (type == typeof(Post) && entity is Post post)
        {
            var newLike = new Like()
            {
                AppUserId = userId,
                PostId = post.Id,
            };

            await dbContext.Likes.AddAsync(newLike, cancellationToken);
            entity.LikeCount++;
            await dbContext.SaveChangesAsync(cancellationToken);

            return newLike;
        }
        else if (type == typeof(Comment) && entity is Comment comment)
        {
            var newLike = new Like()
            {
                AppUserId = userId,
                CommentId = comment.Id,
            };

            await dbContext.Likes.AddAsync(newLike, cancellationToken);
            entity.LikeCount++;
            await dbContext.SaveChangesAsync(cancellationToken);

            return newLike;
        }

        return null;
    }

    private async Task<List<Like>> GetPaginatedLikesAsync(IQueryable<Like> query,
        PaginationRequest paginationRequest, CancellationToken cancellationToken)
    {
        var paginatedLikes = await query
            .OrderByDescending(l => l.Timestamp)
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return [..paginatedLikes];
    }
    #endregion
}