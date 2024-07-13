using Application.Common.Requests;
using Application.Common.Responses;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Common.Interfaces;

public interface ILikesRepository
{
    /// <summary>
    /// Check to see if the given entity is liked already or not by the user
    /// </summary>
    /// <param name="userId">The user for whom to check if the entity is liked by or not</param>
    /// <param name="entity">The likeable entity to check if it's liked by the user</param>
    /// <param name="cancellationToken">A cancellation token to signal the cancellation if the operation was cancelled</param>
    /// <returns>True - if the user has liked the checked entity </returns>
    /// <returns>False - if the user doesn't have a like for the checked entity</returns>
    Task<bool> IsLikedAsync(Guid userId, ILikeable entity, CancellationToken cancellationToken);

    /// <summary>
    /// Performs a like on the given entity
    /// </summary>
    /// <param name="userId">The user who is performing the like/unlike</param>
    /// <param name="entity">The likeable entity that the user has liked/unliked</param>
    /// <param name="cancellationToken">A cancellation token to signal the canellation if the operation was cancelled</param>
    /// <returns>The saved like in the database or Null if the entity is already liked, this will remove the like from the database
    /// and unlike the post</returns>
    Task<Like?> LikeAsync(Guid userId, ILikeable entity, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a paginated readonly collection of likes for a given likeable entity
    /// </summary>
    /// <param name="entity">The likeable entity to retrieve the likes for</param>
    /// <param name="paginationRequest">A pagination request carrying information about the Page Number - Page Size - Search Term</param>
    /// <param name="cancellationToken">A cancellation token to signal the canellation if the operation was cancelled</param>
    /// <returns>A paginated readonly collection of likes</returns>
    Task<PaginatedReadOnlyCollection<Like>> GetLikesAsync
        (ILikeable entity, PaginationRequest paginationRequest, CancellationToken cancellationToken);
}