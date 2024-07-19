using Application.Common.Interfaces;
using Application.Common.Mappers;
using Application.Features.Comments.Commands;
using Domain.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Comments.Handlers;

public class CreateCommentCommandHandler
    (
        ICommentsRepository commentsRepository,
        IValidator<Comment> validator
    )
    : IRequestHandler<CreateCommentCommand, Comment>
{
    public async Task<Comment> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = request.ToComment();
        var validationResult = await validator.ValidateAsync(comment, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var result = await commentsRepository
            .SaveCommentAsync
            (
                request.PostId,
                comment,
                cancellationToken
            );

        return result;
    }
}