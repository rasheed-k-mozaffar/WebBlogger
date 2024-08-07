using Application.Common.Interfaces;
using FluentValidation.Results;
using Application.Features.Posts.Commands;
using Application.Features.Posts.Handlers;
using Domain.Enums;
using FluentValidation;
using MediatR;
using NSubstitute.Extensions;

namespace WebBlogger.Tests.Application.Commands.Posts;

public class CreatePostCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreatePost_WhenCommandIsValid()
    {
        // ARRANGE
        var postValidator = Substitute.For<IValidator<Post>>();

        var command = new CreatePostCommand
        (
            Guid.NewGuid(),
            "Valid Title",
            "Valid Content",
            PostStatus.Published,
            new List<Tag>()
        );

        var postsRepository = Substitute.For<IPostsRepository>();

        var validationResult = new ValidationResult();
        postValidator.ValidateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(validationResult));

        var handler = new global::Application.Features.Posts.Handlers.CreatePostCommandHandler(postsRepository, postValidator);

        // ACT
        var result = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        result.Should().Be(Unit.Value);
        await postValidator.Received(1).ValidateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
        await postsRepository.Received(1).SavePostAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenPostIsInvalid()
    {
        // ARRANGE
        var postsRepository = Substitute.For<IPostsRepository>();
        var postValidator = Substitute.For<IValidator<Post>>();
        var command = new CreatePostCommand
        (
            Guid.NewGuid(),
            "",
            "",
            PostStatus.Published,
            new List<Tag>()
        );

        List<ValidationFailure> failures =
        [
            new ValidationFailure("Title", "Title is required"),
            new ValidationFailure("Content", "Content is required")
        ];

        var validationResult = new ValidationResult(failures);

        postValidator.ValidateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(validationResult));

        var handler = new global::Application.Features.Posts.Handlers.CreatePostCommandHandler(postsRepository, postValidator);

        // ACT
        Func<Task> action = async () =>  await handler.Handle(command, CancellationToken.None);

        //ASSERT
        await action.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "Title" && e.ErrorMessage == "Title is required")
                         && ex.Errors.Any(e => e.PropertyName == "Content" && e.ErrorMessage == "Content is required"));

        await postValidator.Received(1).ValidateAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
        await postsRepository.DidNotReceive().SavePostAsync(Arg.Any<Post>(), Arg.Any<CancellationToken>());
    }
}