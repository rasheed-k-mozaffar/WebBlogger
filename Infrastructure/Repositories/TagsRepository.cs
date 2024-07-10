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

public class TagsRepository(AppDbContext context) : ITagsRepository
{
    public async Task<Tag> SaveTagAsync(Tag tag, CancellationToken cancellationToken)
    {
        context.Tags.Add(tag);
        await context.SaveChangesAsync(cancellationToken);

        return tag;
    }

    public async Task<IReadOnlyCollection<Tag>> GetTagsAsync(CancellationToken cancellationToken)
    {
        var tags = await context.Tags.ToListAsync(cancellationToken);
        return tags.AsReadOnly();
    }

    public async Task<bool> DeleteTagAsync(Guid tagId, CancellationToken cancellationToken)
    {
        var tagToDelete = await context.Tags.FindAsync(tagId);

        if (tagToDelete is null)
            throw new TagNotFoundException("The tag you're trying to delete was not found");

        context.Tags.Remove(tagToDelete);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<Tag> UpdateTagAsync(Guid tagId, Tag tag, CancellationToken cancellationToken)
    {
        var tagToUpdate = await context.Tags.FindAsync(tagId);

        if (tagToUpdate is null)
            throw new TagNotFoundException("The tag you're trying to update was not found");

        tagToUpdate.Name = tag.Name;
        tagToUpdate.Description = tag.Description;
        tagToUpdate.CoverImageUrl = tag.CoverImageUrl;

        await context.SaveChangesAsync(cancellationToken);
        return tagToUpdate;
    }

    public async Task<PaginatedReadOnlyCollection<Post>> GetPostsByTagAsync
        (Guid tagId, PaginationRequest<PostSortOption> paginationRequest, CancellationToken cancellationToken)
    {
        var tag = context
            .Tags
            .Include(t => t.Posts)
            .Single(t => t.Id == tagId);

        if (tag.Posts is null)
            return PaginatedReadOnlyCollection<Post>
                .Empty(paginationRequest.PageNumber, paginationRequest.PageSize);

        var query = tag.Posts.AsQueryable();

        var filteredQuery = query.ApplyFilter(paginationRequest.SearchTerm);
        var sortedQuery = filteredQuery.ApplySorting(paginationRequest.SortOption);

        var totalCount = filteredQuery.Count();
        var posts = sortedQuery
            .ApplyPagination(paginationRequest.PageNumber, paginationRequest.PageSize)
            .ToList();

        var paginatedPosts = new PaginatedReadOnlyCollection<Post>(
            totalCount,
            paginationRequest.PageNumber,
            paginationRequest.PageSize,
            posts.AsReadOnly()
        );

        return paginatedPosts;
    }

    public async Task<Tag> GetTagByIdAsync(Guid tagId, CancellationToken cancellationToken)
    {
        var tag = await context
        .Tags
        .FirstOrDefaultAsync(t => t.Id == tagId, cancellationToken);

        if (tag is null)
        {
            throw new TagNotFoundException("No tag was found with the given ID");
        }

        return tag;
    }
}