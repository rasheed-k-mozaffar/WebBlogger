using System.Collections.ObjectModel;
using Application.Common.Interfaces;
using Application.Features.Tags.Handlers;
using Application.Features.Tags.Queries;
using NSubstitute.ExceptionExtensions;

namespace WebBlogger.Tests.Application.Queries.Tags;

public class GetAllTagsQueryTests
{
    private readonly ITagsRepository _tagsRepository;
    private readonly GetAllTagsQueryHandler _queryHandler;

    public GetAllTagsQueryTests()
    {
        _tagsRepository = Substitute.For<ITagsRepository>();
        _queryHandler = new GetAllTagsQueryHandler(_tagsRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllTagsAsReadonlyCollection()
    {
        // ARRANGE
        var query = new GetAllTagsQuery();
        var allTags = new List<Tag>()
        {
            new Tag() { Id = Guid.NewGuid(), Name = "C#" },
            new Tag() { Id = Guid.NewGuid(), Name = "ASP.NET Core" },
            new Tag() { Id = Guid.NewGuid(), Name = "Blazor" }
        };
        _queryHandler.Handle(query, CancellationToken.None)
            .Returns(allTags.AsReadOnly());

        // ACT
        var result = await _queryHandler
            .Handle(query, CancellationToken.None);

        // ASSERT
        result.Should().BeEquivalentTo(allTags);
        result.Should().BeOfType<ReadOnlyCollection<Tag>>();
        result.Count.Should().Be(allTags.Count);
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCanceledException_WhenCancellationIsRequested()
    {
        // ARRANGE
        var query = new GetAllTagsQuery();
        var cts = new CancellationTokenSource();

        _tagsRepository
            .GetTagsAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(ci => throw new OperationCanceledException());

        // ACT
        await cts.CancelAsync();
        Func<Task> action = async () => await _queryHandler.Handle(query, cts.Token);

        // ASSERT
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
}