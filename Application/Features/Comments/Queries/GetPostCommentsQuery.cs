using Application.Common.Enums;
using Application.Common.Responses;
using Domain.Models;
using MediatR;

namespace Application.Features.Comments.Queries;

public record GetPostCommentsQuery
(
    Guid PostId,
    int PageNumber,
    int PageSize,
    CommentsSortOption SortOption
) : IRequest<PaginatedListCollection<Comment>>;