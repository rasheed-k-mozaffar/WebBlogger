using Application.Common.Interfaces;
using Application.Common.Mappers;
using Application.Features.Posts.Commands;
using Domain.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Posts.Handlers;

public class UpdatePostCommandHandler(IPostsRepository postsRepository, IValidator<Post> validator)
    : IRequestHandler<UpdatePostCommand, Post?>
{
    public async Task<Post?> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = request.MapToPost();

        var validationResult = await validator.ValidateAsync(post, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var updatedPost = await postsRepository.UpdatePostAsync(post.Id, post, cancellationToken);
        return updatedPost;
    }
}