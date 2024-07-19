using Application.Common.Interfaces;
using Application.Common.Mappers;
using Application.Features.Comments.Commands;
using Domain.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Comments.Handlers;

public class UpdateCommentCommandHandler
    (
        ICommentsRepository commentsRepository,
        IValidator<Comment> validator
    )
    : IRequestHandler<UpdateCommentCommand, Comment>
{
    public async Task<Comment> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = request.ToComment();
        var validationResult = await validator.ValidateAsync(comment, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var result = await commentsRepository
            .UpdateCommentAsync(request.Id, comment, cancellationToken);

        return result;
    }
}