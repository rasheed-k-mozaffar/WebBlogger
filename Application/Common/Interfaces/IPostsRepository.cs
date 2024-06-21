using Application.Common.Enums;
using Application.Common.Responses;
using Domain.Enums;
using Domain.Models;

namespace Application.Common.Interfaces;

public interface IPostsRepository
{
    Task<Post> SavePostAsync(Post post, CancellationToken cancellationToken);

    Task<PaginatedReadOnlyCollection<Post>> GetLatestPostsAsync(int pageNumber, int pageSize, string? title,
        PostSortOption sortOption,  CancellationToken cancellationToken);

    Task<Post?> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken);

    Task DeletePostAsync(Guid postId, DeleteType deleteType,CancellationToken cancellationToken);

    Task<Post?> UpdatePostAsync(Guid postId, Post post, CancellationToken cancellationToken);
}