using System.Data.Common;
using Application.Common.Enums;
using Application.Common.Interfaces;
using Application.Common.Responses;
using Application.Features.Posts.Handlers;
using Application.Features.Posts.Queries;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace WebBlogger.Tests.Application.Queries.Posts;

public class GetLatestPostsQueryTests
{
    private readonly IPostsRepository _postsRepository;

    public GetLatestPostsQueryTests()
    {
        _postsRepository = Substitute.For<IPostsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCanceledException_WhenCancellationIsRequested()
    {
        // ARRANGE
        CancellationTokenSource cts = new();
        var expected = PaginatedReadOnlyCollection<Post>.Empty(0, 0);

        // Simulate cancellation by throwing TaskCanceledException when the token is canceled
        _postsRepository.GetLatestPostsAsync(1, 10, null, PostSortOption.MostRecent, Arg.Any<CancellationToken>())
            .Returns( ci =>
            {
                ci.Arg<CancellationToken>().ThrowIfCancellationRequested();
                return expected;
            });

        var handler = new GetLatestPostsQueryHandler(_postsRepository);
        var query = new GetLatestPostsQuery(1, 10, PostSortOption.MostRecent);

        // ACT
        cts.Cancel();
        Func<Task> action = async () => await handler.Handle(query, cts.Token);

        // ASSERT
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
}