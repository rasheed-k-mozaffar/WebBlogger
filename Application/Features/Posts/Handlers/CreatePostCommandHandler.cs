using Application.Common.Interfaces;
using Application.Common.Mappers;
using Application.Features.Posts.Commands;
using Application.Validators;
using Domain.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Posts.Handlers;

public class CreatePostCommandHandler(IPostsRepository postsRepository, IValidator<Post> validator)
    : IRequestHandler<Commands.CreatePostCommand, Unit>
{
    public async Task<Unit> Handle(Commands.CreatePostCommand request, CancellationToken cancellationToken)
    {
        Post post = request.MapToPost();
        var validationResult = await validator.ValidateAsync(post, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        await postsRepository.SavePostAsync(post, cancellationToken);
        return Unit.Value;
    }
}