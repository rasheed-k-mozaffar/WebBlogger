using Application.Common.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;

public static class PostsQueryExtensions
{
    public static IQueryable<Post> ApplyFilter(this IQueryable<Post> query, string? title)
    {
        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(post => post.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        return query;
    }

    public static IOrderedQueryable<Post> ApplySorting(this IQueryable<Post> query, PostSortOption sortOption)
    {
        return sortOption switch
        {
            PostSortOption.MostComments => query.OrderByDescending(p => p.Comments.Count),
            PostSortOption.MostLiked => query.OrderByDescending(p => p.LikeCount),
            PostSortOption.MostViews => query.OrderByDescending(p => p.Views),
            _ => query.OrderByDescending(p => p.PublishedOn)
        };
    }

    public static IQueryable<Post> ApplyPagination(this IQueryable<Post> query, int pageNumber, int pageSize)
    {
        return query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    public static async Task<List<Post>> Execute(this IQueryable<Post> query, CancellationToken cancellationToken)
        => await query.ToListAsync(cancellationToken);
}