using Application.Common.Interfaces;
using Application.Features.Tags.Commands;
using Application.Features.Tags.Handlers;
using Domain.Exceptions;
using NSubstitute.ExceptionExtensions;

namespace WebBlogger.Tests.Application.Commands.Tags;

public class DeleteTagCommandHandlerTests
{
    private readonly ITagsRepository _tagsRepository;
    private readonly DeleteTagCommandHandler _handler;

    public DeleteTagCommandHandlerTests()
    {
        _tagsRepository = Substitute.For<ITagsRepository>();
        _handler = new DeleteTagCommandHandler(_tagsRepository);
    }

    [Fact]
    public async Task Handle_ShouldDeleteTag_WhenGivenTagIdExists()
    {
        // ARRANGE
        var command = new DeleteTagCommand(Guid.NewGuid());
        _tagsRepository.DeleteTagAsync(Arg.Any<Guid>(), CancellationToken.None)
            .Returns(true);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        result.Should().Be(true);
        await _tagsRepository.Received(1)
            .DeleteTagAsync(Arg.Any<Guid>(), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldThrowTagNotFoundException_WhenGivenTagIdDoesNotExist()
    {
        // ARRANGE
        var command = new DeleteTagCommand(Guid.NewGuid());

        _tagsRepository.DeleteTagAsync(Arg.Any<Guid>(), CancellationToken.None)
            .ThrowsAsync(new TagNotFoundException("No tag was found with the given ID"));

        // ACT
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        await action.Should().ThrowAsync<TagNotFoundException>();
        await _tagsRepository.Received(1).DeleteTagAsync(Arg.Any<Guid>(), CancellationToken.None);
    }
}