using Application.Common.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;

public static class CommentsQueryExtensions
{
    public static IQueryable<Comment> ApplyFilter(this IQueryable<Comment> query, string? title)
    {
        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(comment => comment.Content.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        return query;
    }

    public static IOrderedQueryable<Comment> ApplySorting(this IQueryable<Comment> query, CommentsSortOption sortOption)
    {
        return sortOption switch
        {
            CommentsSortOption.MostReplies => query.OrderByDescending(p => p.Replies!.Count),
            CommentsSortOption.MostLiked => query.OrderByDescending(p => p.LikeCount),
            _ => query.OrderByDescending(p => p.WrittenOn)
        };
    }

    public static IQueryable<Comment> ApplyPagination(this IQueryable<Comment> query, int pageNumber, int pageSize)
    {
        return query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    public static async Task<List<Comment>> Execute(this IQueryable<Comment> query, CancellationToken cancellationToken)
        => await query.ToListAsync(cancellationToken);
}