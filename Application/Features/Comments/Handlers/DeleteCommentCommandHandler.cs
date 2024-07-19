using Application.Common.Interfaces;
using Application.Features.Comments.Commands;
using Domain.Models;
using MediatR;

namespace Application.Features.Comments.Handlers;

public class DeleteCommentCommandHandler(ICommentsRepository commentsRepository)
    : IRequestHandler<DeleteCommentCommand, bool>
{
    public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        return await commentsRepository
            .DeleteCommentAsync(request.Id, cancellationToken);;
    }
}