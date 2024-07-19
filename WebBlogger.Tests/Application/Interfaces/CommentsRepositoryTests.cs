using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Requests;
using Application.Common.Responses;
using Domain.Exceptions;
using NSubstitute.ExceptionExtensions;

namespace WebBlogger.Tests.Application.Interfaces;

public class CommentsRepositoryTests
{
    #region Test Variables
    private readonly ICommentsRepository _commentsRepository;
    private readonly Guid _postId;
    private readonly Guid _parentCommentId;
    private readonly PaginationRequest<CommentsSortOption> _paginationRequest;
    private readonly List<Comment> _comments;
    #endregion

    public CommentsRepositoryTests()
    {
        _commentsRepository = Substitute.For<ICommentsRepository>();
        _postId = Guid.NewGuid();
        _parentCommentId = Guid.NewGuid();
        _paginationRequest = new(pageNumber: 1, pageSize: 3, CommentsSortOption.MostRecent);
        _comments = [
            new Comment()
            {
                Id = Guid.NewGuid(),
                PostId = _postId,
                Content = "Comment 1",
            },

            new Comment()
            {
                Id = Guid.NewGuid(),
                PostId = _postId,
                Content = "Comment 2",
            },

            new Comment()
            {
                Id = Guid.NewGuid(),
                PostId = _postId,
                Content = "Comment 3",
            }
        ];
    }

    #region GetCommentsAsync
    [Fact]
    public async Task GetCommentsAsync_ShouldReturnPaginatedListOfComments()
    {
        // ARRANGE
        var paginatedComments = new PaginatedListCollection<Comment>
        (
            totalCount: 3,
            pageSize: _paginationRequest.PageSize,
            pageNumber: _paginationRequest.PageNumber,
            items: _comments
        );
        _commentsRepository.GetCommentsAsync(_postId, _paginationRequest, CancellationToken.None)
            .Returns(paginatedComments);

        // ACT
        var result = await _commentsRepository
            .GetCommentsAsync(_postId, _paginationRequest, CancellationToken.None);

        // ASSERT
        result.Should().BeEquivalentTo(paginatedComments);
        result.Items.Count().Should().Be(_paginationRequest.PageSize);
    }

    [Fact]
    public async Task GetCommentsAsync_ShouldReturnEmptyPaginatedListOfComments()
    {
        // ARRANGE
        var emptyPaginatedCommentsList = PaginatedListCollection<Comment>
            .Empty(_paginationRequest.PageNumber, _paginationRequest.PageSize);

        _commentsRepository.GetCommentsAsync(_postId, _paginationRequest, CancellationToken.None)
            .Returns(emptyPaginatedCommentsList);

        // ACT
        var result = await _commentsRepository
            .GetCommentsAsync(_postId, _paginationRequest, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Items.Count().Should().Be(0);
    }

    [Fact]
    public async Task GetCommentsAsync_ShouldThrowOperationCancelledException_WhenCancellationIsRequested()
    {
        // ARRANGE
        var cts = new CancellationTokenSource();
        _commentsRepository.GetCommentsAsync(_postId, _paginationRequest, cts.Token)
            .ThrowsAsync(new OperationCanceledException());

        await cts.CancelAsync();
        Func<Task> action = async () => await _commentsRepository
            .GetCommentsAsync(_postId, _paginationRequest, cts.Token);

        // ACT & ASSERT
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
    #endregion

    # region SaveCommentAsync
    [Fact]
    public async Task SaveCommentAsync_ShouldReturnSavedComment()
    {
        // ARRANGE
        var comment = new Comment()
        {
            Id = Guid.NewGuid(),
            PostId = _postId,
            Content = "Test comment"
        };

        _commentsRepository.SaveCommentAsync(_postId, comment, CancellationToken.None)
            .Returns(comment);

        // ACT
        var result = await _commentsRepository.SaveCommentAsync(_postId, comment, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Id.Should().Be(comment.Id);
    }

    [Fact]
    public async Task SaveCommentAsync_ShouldThrowOperationCancelledException_WhenCancellationIsRequested()
    {
        // ARRANGE
        var cts = new CancellationTokenSource();
        var comment = new Comment();

        _commentsRepository.SaveCommentAsync(_postId, comment, cts.Token)
            .ThrowsAsync(new OperationCanceledException());

        await cts.CancelAsync();
        Func<Task> action = async () => await _commentsRepository
            .SaveCommentAsync(_postId, comment, cts.Token);

        // ACT & ASSERT
        await action.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task SaveCommentAsync_ShouldReturnSavedCommentWithParentCommentId_WhenParentCommentIdIsNotNull()
    {
        // ARRANGE
        var reply = new Comment()
        {
            Id = Guid.NewGuid(),
            PostId = _postId,
            ParentCommentId = _parentCommentId,
            Content = "Test reply comment"
        };

        _commentsRepository.SaveCommentAsync(_postId, reply, CancellationToken.None)
            .Returns(reply);

        // ACT
        var result = await _commentsRepository
            .SaveCommentAsync(_postId, reply, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Id.Should().Be(reply.Id);
        result.ParentCommentId.Should().Be(reply.ParentCommentId);
    }

    [Fact]
    public async Task SaveCommentAsync_ShouldThrowCommentNotFoundException_WhenParentCommentIdDoesNotExist()
    {
        // ARRANGE
        var reply = new Comment()
        {
            Id = Guid.NewGuid(),
            PostId = _postId,
            ParentCommentId = Guid.NewGuid(),
            Content = "Test reply comment"
        };

        _commentsRepository.SaveCommentAsync(_postId, reply, CancellationToken.None)
            .ThrowsAsync(new CommentNotFoundException("The comment you are replying to was not found"));

        Func<Task> action = async() => await _commentsRepository
            .SaveCommentAsync(_postId, reply, CancellationToken.None);

        // ACT & ASSERT
        await action.Should().ThrowAsync<CommentNotFoundException>();

    }

    [Fact]
    public async Task SaveCommentAsync_ShouldThrowPostNotFoundException_WhenPostIdDoesNotExist()
    {
        // ARRANGE
        var comment = new Comment()
        {
            Id = Guid.NewGuid(),
            PostId = Guid.NewGuid(),
            Content = "Test comment"
        };

        _commentsRepository.SaveCommentAsync(comment.PostId, comment, CancellationToken.None)
            .ThrowsAsync(new PostNotFoundException("The post you're trying to comment on was not found"));

        Func<Task> action = async () => await _commentsRepository
            .SaveCommentAsync(comment.PostId, comment, CancellationToken.None);

        // ACT & ASSERT
        await action.Should().ThrowAsync<PostNotFoundException>();
    }
    # endregion

    #region DeleteCommentAsync
    [Fact]
    public async Task DeleteCommentAsync_ShouldReturnTrue_WhenCommentIsDeleted()
    {
        // ARRANGE
        var commentId = Guid.NewGuid();
        _commentsRepository.DeleteCommentAsync(commentId, Arg.Any<CancellationToken>())
            .Returns(true);

        // ACT
        var result = await _commentsRepository
            .DeleteCommentAsync(commentId, CancellationToken.None);

        // ASSERT
        result.Should().BeTrue();
        await _commentsRepository.Received(1).DeleteCommentAsync(commentId, CancellationToken.None);
    }

    [Fact]
    public async Task DeleteCommentAsync_ShouldThrowCommentNotFoundException_WhenCommentToDeleteDoesNotExist()
    {
        _commentsRepository.DeleteCommentAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new CommentNotFoundException("The comment you're trying to delete was not found"));

        Func<Task> action = async () =>
            await _commentsRepository.DeleteCommentAsync(Guid.NewGuid(), CancellationToken.None);

        // ACT & ASSERT
        await action.Should().ThrowAsync<CommentNotFoundException>();
        await _commentsRepository.Received(1).DeleteCommentAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCommentAsync_ShouldThrowOperationCancelledException_WhenCancellationIsRequested()
    {
        // ARRANGE
        var cts = new CancellationTokenSource();
        _commentsRepository.DeleteCommentAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        Func<Task> action = async () =>
            await _commentsRepository.DeleteCommentAsync(Guid.NewGuid(), cts.Token);

        // ACT & ASSERT
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
    #endregion

    #region GetCommentByIdAsync

    [Fact]
    public async Task GetCommentByIdAsync_ShouldReturnCorrectComment_WhenCommentIsFound()
    {
        // ARRANGE
        var commentId = Guid.NewGuid();
        var comment = new Comment()
        {
            Id = commentId,
            PostId = _postId,
            Content = "Test comment"
        };

        _commentsRepository.GetCommentByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(comment);

        // ACT
        var result = await _commentsRepository
            .GetCommentByIdAsync(commentId, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(comment);
    }

    [Fact]
    public async Task GetCommentByIdAsync_ShouldThrowCommentNotFoundException_WhenCommentIdDoesNotExist()
    {
        // ARRANGE
        _commentsRepository.GetCommentByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new CommentNotFoundException("No comment was found with the given ID"));

        Func<Task> action = async() => await _commentsRepository
            .GetCommentByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // ACT & ASSERT
        await action.Should().ThrowAsync<CommentNotFoundException>();
    }

    [Fact]
    public async Task GetCommentByIdAsync_ShouldThrowOperationCancelledException_WhenCancellationIsRequested()
    {
        // ARRANGE
        _commentsRepository.GetCommentByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        Func<Task> action = async () => await _commentsRepository
            .GetCommentByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // ACT & ASSERT
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
    #endregion

    #region UpdateCommentAsync
    [Fact]
    public async Task UpdateCommentAsync_ShouldReturnUpdatedComment_WhenCommentIsUpdatedCorrectly()
    {
        // ARRANGE
        var commentId = Guid.NewGuid();

        var updatedComment = new Comment()
        {
            Id = commentId,
            PostId = _postId,
            Content = "Updated test comment"
        };

        _commentsRepository.UpdateCommentAsync(Arg.Any<Guid>(), Arg.Any<Comment>(), Arg.Any<CancellationToken>())
            .Returns(updatedComment);

        // ACT
        var result = await _commentsRepository
            .UpdateCommentAsync(commentId, updatedComment, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(updatedComment);
    }

    [Fact]
    public async Task UpdateCommentAsync_ShouldThrowCommentNotFoundException_WhenCommentIdDoesNotExist()
    {
        // ARRANGE
        _commentsRepository.UpdateCommentAsync(Arg.Any<Guid>(), Arg.Any<Comment>(),Arg.Any<CancellationToken>())
            .ThrowsAsync(new CommentNotFoundException("The comment you're trying to update was not found"));

        Func<Task> action = async () => await _commentsRepository
            .UpdateCommentAsync(Guid.NewGuid(), new Comment(), CancellationToken.None);

        // ACT & ASSERT
        await action.Should().ThrowAsync<CommentNotFoundException>();
    }

    [Fact]
    public async Task UpdateCommentAsync_ShouldThrowOperationCancelledException_WhenCancellationIsRequested()
    {
        // ARRANGE
        _commentsRepository.UpdateCommentAsync(Arg.Any<Guid>(), Arg.Any<Comment>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        Func<Task> action = async () => await _commentsRepository
            .UpdateCommentAsync(Guid.NewGuid(), new Comment(), CancellationToken.None);

        // ACT & ASSERT
        await action.Should().ThrowAsync<OperationCanceledException>();

    }
    #endregion
}