using Application.Common.Options;
using Domain.Exceptions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using WebBlogger.Tests.Shared;

namespace WebBlogger.Tests.Infrastructure.Repositories;

public class CommentsRepositoryTests : IAsyncDisposable
{
    private const string InMemDatabaseName = "in-mem-db";

    private readonly AppDbContext _dbContext;
    private readonly CommentsRepository _commentsRepository;

    public CommentsRepositoryTests()
    {
        _dbContext = new AppDbContext(
                DatabaseContextOptionsBuilder.CreateNewContextOptions(InMemDatabaseName),
                new EntityFrameworkOptions()
            );

        _commentsRepository = new CommentsRepository(_dbContext);
    }

    [Fact]
    public async Task SaveComment_ShouldSaveComment()
    {
        // ARRANGE
        InMemoryDatabaseSeeder.SeedPosts(_dbContext, 1);
        var postToAddCommentTo = await _dbContext.Posts.FirstAsync();

        var comment = new Comment()
        {
            Id = Guid.NewGuid(),
            PostId = postToAddCommentTo.Id,
            ParentCommentId = null,
            Post = postToAddCommentTo,
            Content = "Test reply comment"
        };

        // ACT
        var result = await _commentsRepository
            .SaveCommentAsync
            (
                postToAddCommentTo.Id,
                comment,
                CancellationToken.None
            );

        // ASSERT
        result.Should().NotBeNull();
        result.Id.Should().Be(comment.Id);
        result.PostId.Should().Be(postToAddCommentTo.Id);
    }

    [Fact]
    public async Task SaveComment_ShouldThrowPostNotFoundException_WhenPostIdDoesNotExist()
    {
        // ARRANGE
        var nonExistentPostId = Guid.NewGuid();
        var comment = new Comment()
        {
            Id = Guid.NewGuid(),
            PostId = nonExistentPostId,
            ParentCommentId = null,
            Content = "Test comment"
        };

        Func<Task> action = async () => await _commentsRepository
            .SaveCommentAsync(nonExistentPostId, comment, CancellationToken.None);

        // ACT & ASSERT
        await action.Should().ThrowAsync<PostNotFoundException>();
    }

    [Fact]
    public async Task SaveComment_ShouldSaveReplyComment_WhenParentCommentIdIsNotNull()
    {
        // ARRANGE
        InMemoryDatabaseSeeder.SeedPosts(_dbContext, 1);
        var postToAddCommentTo = await _dbContext.Posts.FirstAsync();
        InMemoryDatabaseSeeder.SeedComments(_dbContext, postToAddCommentTo, 1);
        var commentToReplyTo = await _dbContext.Comments.FirstAsync();

        var reply = new Comment()
        {
            Id = Guid.NewGuid(),
            PostId = postToAddCommentTo.Id,
            ParentCommentId = commentToReplyTo.Id,
            Post = postToAddCommentTo,
            Content = "Test reply"
        };

        // ACT
        var result = await _commentsRepository
            .SaveCommentAsync
            (
                postToAddCommentTo.Id,
                reply,
                CancellationToken.None
            );

        // ASSERT
        result.Should().NotBeNull();
        result.Id.Should().Be(reply.Id);
        result.PostId.Should().Be(postToAddCommentTo.Id);
        result.ParentCommentId.Should().Be(commentToReplyTo.Id);
    }

    [Fact]
    public async Task SaveComment_ShouldThrowCommentNotFoundException_WhenParentCommentIdDoesNotExist()
    {
        // ARRANGE
        InMemoryDatabaseSeeder.SeedPosts(_dbContext, 1);
        var postToAddCommentTo = await _dbContext.Posts.FirstAsync();

        var reply = new Comment()
        {
            Id = Guid.NewGuid(),
            PostId = postToAddCommentTo.Id,
            Post = postToAddCommentTo,
            ParentCommentId = Guid.NewGuid(),
            Content = "Test reply"
        };

        Func<Task> action = async () => await _commentsRepository
            .SaveCommentAsync
            (
                postToAddCommentTo.Id,
                reply,
                CancellationToken.None
            );

        // ACT & ASSERT
        await action.Should().ThrowAsync<CommentNotFoundException>();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}