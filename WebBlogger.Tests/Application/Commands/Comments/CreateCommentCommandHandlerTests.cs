using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.Features.Comments.Commands;
using Application.Features.Comments.Handlers;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute.ExceptionExtensions;

namespace WebBlogger.Tests.Application.Commands.Comments;

public class CreateCommentCommandHandlerTests
{
    private Guid _postId;
    private Guid _commentId;

    private readonly IValidator<Comment> _validator;
    private readonly ICommentsRepository _commentsRepository;
    private readonly CreateCommentCommandHandler _handler;

    public CreateCommentCommandHandlerTests()
    {
        _postId = Guid.NewGuid();
        _commentId = Guid.NewGuid();

        _validator = Substitute.For<IValidator<Comment>>();
        _commentsRepository = Substitute.For<ICommentsRepository>();
        _handler = new CreateCommentCommandHandler(_commentsRepository, _validator);
    }

    [Fact]
    public async Task Handle_ShouldSaveCommentAsync_WhenCommandIsValid()
    {
        // ARRANGE
        var comment = new Comment()
        {
            Id = _commentId,
            PostId = _postId,
            Content = "Valid Comment Content"
        };

        var command = new CreateCommentCommand
        (
            comment.Id,
            comment.PostId,
            null,
            Guid.NewGuid(),
            comment.Content
        );

        _validator.ValidateAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _commentsRepository.SaveCommentAsync
        (
            Arg.Any<Guid>(),
            Arg.Any<Comment>(),
            Arg.Any<CancellationToken>()
        ).Returns(comment);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Id.Should().Be(_commentId);
        result.PostId.Should().Be(_postId);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // ARRANGE
        var command = new CreateCommentCommand
        (
            _commentId,
            _postId,
            null,
            Guid.NewGuid(),
            string.Empty
        );

        List<ValidationFailure> failures =
        [
            new ValidationFailure
            (
                nameof(Comment.Content),
                "The comment cannot be empty"
            )
        ];

        Expression<Func<ValidationException, bool>> contentValidationErrorHasOccured =
            (ValidationException e) =>
            e.Errors.Any(error => error.PropertyName == nameof(Comment.Content));

        _validator.ValidateAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ValidationException(failures));

        Func<Task> action = async() => await _handler.Handle(command, CancellationToken.None);

        // ACT & ASSERT
        await action.Should().ThrowAsync<ValidationException>();
        await action.Should().ThrowAsync<ValidationException>()
            .Where(contentValidationErrorHasOccured);
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCancelledException_WhenCancellationIsRequested()
    {
        // ARRANGE
        var command = new CreateCommentCommand
        (
            _commentId,
            _postId,
            null,
            Guid.NewGuid(),
            "Test comment"
        );
        var cts = new CancellationTokenSource();

        _validator.ValidateAsync(Arg.Any<Comment>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _commentsRepository.SaveCommentAsync
        (
            Arg.Any<Guid>(),
            Arg.Any<Comment>(),
            Arg.Any<CancellationToken>()
        ).ThrowsAsync(new OperationCanceledException());

        await cts.CancelAsync();
        Func<Task> action = async () => await _handler.Handle(command, cts.Token);

        // ACT & ASSERT
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
}