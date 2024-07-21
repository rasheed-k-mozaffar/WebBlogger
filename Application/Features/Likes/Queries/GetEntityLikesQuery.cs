using Application.Common.Requests;
using Application.Common.Responses;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Likes.Queries;

public record GetEntityLikesQuery
(
    ILikeable Entity,
    int PageNumber,
    int PageSize
): IRequest<PaginatedReadOnlyCollection<Like>>;