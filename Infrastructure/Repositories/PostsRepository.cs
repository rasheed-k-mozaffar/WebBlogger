using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Responses;
using Application.Extensions;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories;

public class PostsRepository(AppDbContext db, ILogger<PostsRepository> logger) : IPostsRepository
{
    public async Task<Post> SavePostAsync(Post post, CancellationToken cancellationToken)
    {
        try
        {
            db.Posts.Add(post);
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError("Post saving was canceled");
            throw;
        }

        return post;
    }

    public async Task<PaginatedReadOnlyCollection<Post>>
        GetLatestPostsAsync(int pageNumber, int pageSize, string? title, PostSortOption sortOption, CancellationToken cancellationToken)
    {
        var query = db.Posts.AsQueryable();

        var filteredQuery = query.ApplyFilter(title);
        var sortedQuery = filteredQuery.ApplySorting(sortOption);

        var totalCount = await filteredQuery.CountAsync(cancellationToken);
        var posts = await sortedQuery
            .ApplyPagination(pageNumber, pageSize)
            .Execute(cancellationToken);

        var paginatedPosts = new PaginatedReadOnlyCollection<Post>(
            totalCount,
            pageNumber,
            pageSize,
            posts.AsReadOnly()
        );

        return paginatedPosts;
    }

    public async Task<Post?> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken)
    {
        try
        {
            var post = await db
                .Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(post => post.Id == postId, cancellationToken);

            if (post is null)
                throw new PostNotFoundException("The post you're looking for was not found");

            return post;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Loading post was cancelled");
            return null;
        }
    }

    public async Task DeletePostAsync(Guid postId, DeleteType deleteType,
        CancellationToken cancellationToken)
    {
        var post = await db
            .Posts
            .FindAsync(postId);

        if (post is null)
            throw new PostNotFoundException($"No post was found with the ID: {postId}");

        if (deleteType == DeleteType.Hard)
            db.Posts.Remove(post);
        else if (deleteType == DeleteType.Soft)
            post.Status = PostStatus.Deleted;

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Post> UpdatePostAsync(Guid postId, Post post, CancellationToken cancellationToken)
    {
        var postToUpdate = await db
            .Posts
            .FindAsync(postId);

        if (postToUpdate is null)
            throw new PostNotFoundException($"No post was found with the ID: {postId}");

        postToUpdate.Title = post.Title;
        postToUpdate.Content = post.Content;
        postToUpdate.Status = post.Status;
        postToUpdate.Tags = post.Tags;
        postToUpdate.LastEditedOn = DateTime.UtcNow;
        postToUpdate.Status = post.Status;

        await db.SaveChangesAsync(cancellationToken);
        return postToUpdate;
    }
}