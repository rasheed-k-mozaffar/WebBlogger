using Application.Common.Enums;
using Application.Common.Requests;
using Application.Common.Responses;
using Domain.Models;

namespace Application.Common.Interfaces;

public interface IBookmarkedPostsRepository
{
    /// <summary>
    /// Saves a post to the user's bookmarked posts (Reading list)
    /// </summary>
    /// <param name="userId">The user to bookmark the post for</param>
    /// <param name="postId">The selected post to bookmark</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to signal cancellation of the operation</param>
    /// <returns></returns>
    Task BookmarkPostAsync(Guid userId, Guid postId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a post from the user's bookmarked posts (Reading list)
    /// </summary>
    /// <param name="userId">The user to remove the bookmarked post for</param>
    /// <param name="postId">The selected post to remove from boomarks</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to signal cancellation of the operation</param>
    /// <returns></returns>
    Task RemoveBookmarkedPostAsync(Guid userId, Guid postId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the bookmarked posts for the user
    /// </summary>
    /// <param name="userId">The user to retrieve the bookmarks for</param>
    /// <param name="paginationRequest">A <see cref="PaginationRequest{T}"/> to paginate, sort and filter the returned bookmarks</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to signal cancellation of the operation</param>
    /// <returns></returns>
    Task<PaginatedListCollection<Post>> GetBookmarkedPostsAsync
        (Guid userId, PaginationRequest<PostSortOption> paginationRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Clears the user's bookmarks
    /// </summary>
    /// <param name="userId">The user to clear the bookmarks for</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to signal cancellation of the operation</param>
    /// <returns></returns>
    Task ClearBookmarksAsync(Guid userId, CancellationToken cancellationToken);
}