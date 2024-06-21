using Application.Common.Interfaces;
using Application.Common.Mappers;
using Application.Features.Posts.Commands;
using Application.Validators;
using Domain.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Posts.Handlers;

public class CreatePostCommandHandler(IPostsRepository postsRepository, IValidator<Post> validator)
    : IRequestHandler<CreatePostCommand, Unit>
{
    private readonly IPostsRepository _postsRepository = postsRepository;
    private readonly IValidator<Post> _validator = validator;

    public async Task<Unit> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        Post post = request.MapToPost();
        var validationResult = await _validator.ValidateAsync(post, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        await _postsRepository.SavePostAsync(post, cancellationToken);
        return Unit.Value;
    }
}