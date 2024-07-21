using Application.Common.Interfaces;
using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Likes.Queries;
using Domain.Models;
using MediatR;

namespace Application.Features.Likes.Handlers;

public class GetEntityLikesQueryHandler(ILikesRepository likesRepository)
    : IRequestHandler<GetEntityLikesQuery, PaginatedReadOnlyCollection<Like>>
{
    public async Task<PaginatedReadOnlyCollection<Like>> Handle(GetEntityLikesQuery request, CancellationToken cancellationToken)
    {
        var paginationRequest = new PaginationRequest(request.PageNumber, request.PageSize);
        var result = await likesRepository
            .GetLikesAsync
            (
                request.Entity,
                paginationRequest,
                cancellationToken
            );

        return result;
    }
}