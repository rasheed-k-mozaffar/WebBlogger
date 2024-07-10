using Application.Common.Interfaces;
using Application.Features.Tags.Handlers;
using Application.Features.Tags.Queries;
using Domain.Exceptions;
using NSubstitute.ExceptionExtensions;

namespace WebBlogger.Tests.Application.Queries.Tags;

public class GetTagByIdQueryTests
{
    private readonly ITagsRepository _tagsRepository;
    private readonly GetTagByIdQueryHandler _queryHandler;

    public GetTagByIdQueryTests()
    {
        _tagsRepository = Substitute.For<ITagsRepository>();
        _queryHandler = new GetTagByIdQueryHandler(_tagsRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnTag_WhenGivenTagIdExists()
    {
        // ARRANGE
        var tag = new Tag()
        {
            Id = Guid.NewGuid(),
            Name = "C#"
        };
        var query = new GetTagByIdQuery(tag.Id);
        _tagsRepository.GetTagByIdAsync(tag.Id, CancellationToken.None)
            .Returns(tag);

        // ACT
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        await _tagsRepository.Received(1).GetTagByIdAsync(tag.Id, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldThrowTagNotFoundException_WhenGivenTagIdDoesNotExist()
    {
        // ARRANGE
        var query = new GetTagByIdQuery(Guid.NewGuid());
        _tagsRepository.GetTagByIdAsync(query.Id, CancellationToken.None)
            .ThrowsAsync(ci =>
                throw new TagNotFoundException($"No tag was found with the ID: {query.Id}"));
        // ACT
        Func<Task> action = async () => await _queryHandler
            .Handle(query, CancellationToken.None);;

        // ASSERT
        await action.Should().ThrowAsync<TagNotFoundException>();
        await _tagsRepository.Received(1).GetTagByIdAsync(query.Id, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenGivenNullQuery()
    {
        // ACT
        Func<Task> action = async () => await _queryHandler
            .Handle(null, CancellationToken.None);

        // ASSERT
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowOperationCancelledException_WhenCancellationWasRequested()
    {
        // ARRANGE
        var query = new GetTagByIdQuery(Guid.NewGuid());
        var cts = new CancellationTokenSource();
        _tagsRepository.GetTagByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        // ACT
        await cts.CancelAsync();
        Func<Task> action = async()  => await _queryHandler.Handle(query, cts.Token);

        // ASSERT
        await action.Should().ThrowAsync<OperationCanceledException>();
    }
}