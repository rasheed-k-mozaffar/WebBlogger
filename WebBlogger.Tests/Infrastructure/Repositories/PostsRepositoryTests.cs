using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Options;
using Application.Common.Responses;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebBlogger.Tests.Shared;

namespace WebBlogger.Tests.Infrastructure.Repositories;

public class PostsRepositoryTests
{
    private readonly ILogger<PostsRepository> _logger;
    private const string InMemDatabaseName = "in-mem-db";

    public PostsRepositoryTests()
    {
        _logger = Substitute.For<ILogger<PostsRepository>>();
    }

    [Fact]
    public async Task SavePostAsync_ShouldReturnSavedPost()
    {
        // ARRANGE
        await using var dbContext = new AppDbContext(DatabaseContextOptionsBuilder
            .CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());
        var postsRepository = new PostsRepository(dbContext, _logger);

        var post = new Post()
        {
            Id = Guid.NewGuid(),
            Title = "Post title",
            Content = "Post content",
            PublishedOn = DateTime.UtcNow,
            Tags = null,
            Status = PostStatus.Published
        };

        // ACT
        var result = await postsRepository.SavePostAsync(post, CancellationToken.None);

        // ASSERT
        var savedPost = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == post.Id);
        savedPost.Should().NotBeNull();
        savedPost!.Id.Should().Be(post.Id);
        savedPost.Title.Should().Be(post.Title);
    }

    [Fact]
    public async Task SavePostAsync_ShouldThrowOperationCancelledException_WhenCancellationIsRequested()
    {
        // ARRANGE
        await using var dbContext = new AppDbContext(DatabaseContextOptionsBuilder
            .CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());

        var postsRepository = new PostsRepository(dbContext, _logger);

        var post = new Post()
        {
            Id = Guid.NewGuid(),
            Title = "Post title",
            Content = "Post content",
            PublishedOn = DateTime.UtcNow,
            Tags = null,
            Status = PostStatus.Published
        };

        var cts = new CancellationTokenSource(1000);
        await Task.Delay(1000);
        // ACT

        Func<Task<Post>> action = async () => await postsRepository.SavePostAsync(post, cts.Token);

        // ASSERT
        var savedPost = await dbContext.Posts.FirstOrDefaultAsync(p => p.Id == post.Id);
        await action.Should().ThrowAsync<OperationCanceledException>();
        savedPost.Should().BeNull();
    }

    [Fact]
    public async Task GetPostById_ShouldThrowPostNotFoundException_WhenPostIsNonExistent()
    {
        // ARRANGE
        await using var context = new AppDbContext(DatabaseContextOptionsBuilder
            .CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());

        var postsRepository = new PostsRepository(context, _logger);

        var nonExistentPostId = Guid.NewGuid();

        // ACT
        Func<Task<Post?>> action = async () =>
            await postsRepository.GetPostByIdAsync(nonExistentPostId, CancellationToken.None);

        // ASSERT
        await action.Should().ThrowAsync<PostNotFoundException>();
        var nonFoundPost = await context.Posts.FindAsync(nonExistentPostId);
        nonFoundPost.Should().BeNull();
    }

    [Theory]
    [InlineData(10, 1)]
    [InlineData(20, 2)]
    public async Task GetLatestPosts_ShouldReturnPaginatedReadOnlyCollection_WithSpecifiedPageSizeAndNumber
        (int pageSize, int pageNumber)
    {
        // ARRANGE
        await using var context = new AppDbContext(DatabaseContextOptionsBuilder
                .CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());

        var postsRepository = new PostsRepository(context, _logger);

        int expectedCount = pageSize;
        int expectedTotalCount = 50;
        bool expectedHasNextPage = true;

        InMemoryDatabaseSeeder.SeedPosts(context, 50);
        // ACT
        var result = await postsRepository
            .GetLatestPostsAsync(pageNumber, pageSize, null, PostSortOption.MostRecent, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Should().BeOfType<PaginatedReadOnlyCollection<Post>>();
        result.Items.Count.Should().Be(expectedCount);
        result.TotalCount.Should().Be(expectedTotalCount);
        result.HasNextPage.Should().Be(expectedHasNextPage);
    }

    [Fact]
    public async Task DeletePost_ShouldChangePostStatusToDeleted_WhenSoftDeleteIsUsed()
    {
        // ARRANGE
        await using var context = new AppDbContext(DatabaseContextOptionsBuilder
            .CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());

        var postsRepository = new PostsRepository(context, _logger);

        InMemoryDatabaseSeeder.SeedPosts(context, 1);

        var postToDelete = context.Posts.First();

        // ACT
        await postsRepository.DeletePostAsync(postToDelete.Id, DeleteType.Soft, CancellationToken.None);

        // ASSERT
        var softDeletedPost = context.Posts.First();
        context.Posts.Count().Should().Be(1);
        softDeletedPost.Status.Should().Be(PostStatus.Deleted);
    }

    [Fact]
    public async Task UpdatePost_ShouldReturnTheUpdatedPost()
    {
        // ARRANGE
        await using var context = new AppDbContext(DatabaseContextOptionsBuilder
            .CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());

        var postsRepository = new PostsRepository(context, _logger);

        InMemoryDatabaseSeeder.SeedPosts(context, 1);

        var postToUpdate = context.Posts.First();
        postToUpdate.Title = "Updated Title!";
        postToUpdate.Content = "Updated Content!";

        // ACT
        var updated = await postsRepository.UpdatePostAsync(postToUpdate.Id, postToUpdate, CancellationToken.None);

        // ASSERT
        updated.Should().NotBeNull();
        updated!.Title.Should().Be(postToUpdate.Title);
        updated.Content.Should().Be(postToUpdate.Content);
    }
}