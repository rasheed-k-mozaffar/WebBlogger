using Application.Common.Interfaces;
using Application.Common.Requests;
using Application.Common.Responses;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class LikesRepository(AppDbContext dbContext) : ILikesRepository
{
    public async Task<bool> IsLikedAsync(Guid userId, ILikeable entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Like?> LikeAsync(Guid userId, ILikeable entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<PaginatedReadOnlyCollection<Like>> GetLikesAsync(ILikeable entity, PaginationRequest paginationRequest, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}