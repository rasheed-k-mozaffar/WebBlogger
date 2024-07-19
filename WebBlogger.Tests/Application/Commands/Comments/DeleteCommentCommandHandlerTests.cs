using Application.Common.Interfaces;
using Application.Features.Comments.Commands;
using Application.Features.Comments.Handlers;
using Domain.Exceptions;
using NSubstitute.ExceptionExtensions;

namespace WebBlogger.Tests.Application.Commands.Comments;

public class DeleteCommentCommandHandlerTests
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly DeleteCommentCommandHandler _handler;

    public DeleteCommentCommandHandlerTests()
    {
        _commentsRepository = Substitute.For<ICommentsRepository>();
        _handler = new DeleteCommentCommandHandler(_commentsRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenCommentIsDeleted()
    {
        // ARRANGE
        var command = new DeleteCommentCommand(Guid.NewGuid());

        _commentsRepository.DeleteCommentAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        result.Should().Be(true);
    }

    [Fact]
    public async Task Handle_ShouldThrowCommentNotFoundException_WhenCommentToDeleteDoesNotExist()
    {
        // ARRANGE
        var command = new DeleteCommentCommand(Guid.NewGuid());
        _commentsRepository.DeleteCommentAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new CommentNotFoundException("No comment was found with the given ID"));

        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // ACT & ASSERT
        await action.Should().ThrowAsync<CommentNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCancelledException_WhenCancellationIsRequested()
    {
        // ARRANGE
        var command = new DeleteCommentCommand(Guid.NewGuid());
        var cts = new CancellationTokenSource();

        _commentsRepository.DeleteCommentAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        Func<Task> action = async () => await _handler.Handle(command, cts.Token);

        // ACT & ASSERT
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
}