using Application.Common.Interfaces;
using Application.Features.Comments.Queries;
using Domain.Models;
using MediatR;

namespace Application.Features.Comments.Handlers;

public class GetCommentByIdQueryHandler(ICommentsRepository commentsRepository)
    : IRequestHandler<GetCommentByIdQuery, Comment?>
{
    public async Task<Comment?> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var comment = await commentsRepository
            .GetCommentByIdAsync(request.Id, cancellationToken);

        return comment;
    }
}