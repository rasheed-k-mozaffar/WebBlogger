using Application.Common.Options;
using Application.Common.Requests;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using WebBlogger.Tests.Shared;

namespace WebBlogger.Tests.Infrastructure.Repositories;

public class LikesRepositoryTests
{
    private const string InMemoryDatabaseName = "in-mem-db";

    private readonly Guid _userId = Guid.NewGuid();
    private readonly AppDbContext _dbContext;
    private readonly LikesRepository _likesRepository;

    public LikesRepositoryTests()
    {
        _dbContext = new AppDbContext(
            DatabaseContextOptionsBuilder.CreateNewContextOptions(InMemoryDatabaseName),
            new EntityFrameworkOptions()
        );

        _likesRepository = new LikesRepository(_dbContext);
    }

    [Fact]
    public async Task LikeAsync_ShouldLikePost_WhenPostIsNotLikedAlreadByTheUser()
    {
        // ARRANGE
        InMemoryDatabaseSeeder.SeedPosts(_dbContext, 1);
        var postToLike = await _dbContext
            .Posts
            .Include(p => p.Likes)
            .FirstAsync();

        // ACT
        var result = await _likesRepository.LikeAsync
        (
            _userId,
            postToLike,
            CancellationToken.None
        );

        // ASSERT
        result.Should().NotBeNull();

        var postLikes = await _likesRepository
            .GetLikesAsync
            (
                postToLike,
                new(1, 10),
                CancellationToken.None
            );

        postLikes.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task LikeAsync_ShouldRemovePostLike_WhenPostIsAlreadyLikedByTheUser()
    {
        // ARRANGE
        InMemoryDatabaseSeeder.SeedPosts(_dbContext, 1);
        var postToUnlike = await _dbContext
            .Posts
            .Include(p => p.Likes)
            .FirstAsync();

        var result = await _likesRepository.LikeAsync
        (
            _userId,
            postToUnlike,
            CancellationToken.None
        );

        // ACT
        result = await _likesRepository.LikeAsync
        (
            _userId,
            postToUnlike,
            CancellationToken.None
        );

        // ASSERT
        result.Should().BeNull();

        var postLikes = await _likesRepository
            .GetLikesAsync
            (
                postToUnlike,
                new(1, 10),
                CancellationToken.None
            );

        postLikes.Items.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetLikesAsync_ShouldReturnPaginatedReadOnlyCollectionOfLikes()
    {
        // ARRANGE
        int pageNumber = 1;
        int pageSize = 20;
        int expectedTotalCount = 50;
        int expectedTotalPages = (int)Math.Ceiling(expectedTotalCount / (double)pageSize);
        PaginationRequest paginationRequest = new(pageNumber, pageSize);

        InMemoryDatabaseSeeder.SeedPosts(_dbContext, 1);
        var post = await _dbContext
            .Posts
            .Include(p => p.Likes)
            .FirstAsync();

        InMemoryDatabaseSeeder.SeedPostLikes(_dbContext, post, 50);

        // ACT
        var result = await _likesRepository
            .GetLikesAsync
            (
                post,
                paginationRequest,
                CancellationToken.None
            );

        // ASSERT
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(pageSize);
        result.TotalCount.Should().Be(expectedTotalCount);
        result.TotalPages.Should().Be(expectedTotalPages);
    }
}