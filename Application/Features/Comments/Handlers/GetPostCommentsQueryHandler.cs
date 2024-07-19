using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Comments.Queries;
using Domain.Models;
using MediatR;

namespace Application.Features.Comments.Handlers;

public class GetPostCommentsQueryHandler(ICommentsRepository commentsRepository)
    : IRequestHandler<GetPostCommentsQuery, PaginatedListCollection<Comment>>
{
    public async Task<PaginatedListCollection<Comment>> Handle(GetPostCommentsQuery request, CancellationToken cancellationToken)
    {
        var result = await commentsRepository
            .GetCommentsAsync
            (
                request.PostId,
                new PaginationRequest<CommentsSortOption>
                (
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize
                ),
                cancellationToken
            );

        return result;
    }
}