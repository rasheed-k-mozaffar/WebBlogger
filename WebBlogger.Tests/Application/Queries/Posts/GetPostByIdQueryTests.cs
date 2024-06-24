using Application.Common.Interfaces;
using Application.Features.Posts.Handlers;
using Application.Features.Posts.Queries;
using Domain.Enums;
using Domain.Exceptions;
using NSubstitute.ExceptionExtensions;

namespace WebBlogger.Tests.Application.Queries.Posts;

public class GetPostByIdQueryTests
{
    private readonly IPostsRepository _postsRepository;

    public GetPostByIdQueryTests()
    {
        _postsRepository = Substitute.For<IPostsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldThrowPostNotFoundException_WhenGivenPostIdDoesNotExist()
    {
        // ARRANGE
        var query = new GetPostByIdQuery(Guid.NewGuid()); // Random ID
        var handler = new GetPostByIdQueryHandler(_postsRepository);

        _postsRepository.GetPostByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Throws(new PostNotFoundException("Post was not found"));
        // ACT
        var action = async () => await handler.Handle(query, CancellationToken.None);

        // ASSERT
        await action.Should().ThrowAsync<PostNotFoundException>()
            .WithMessage("Post was not found");
    }

    [Fact]
    public async Task Handle_ShouldReturnTheCorrectPost_WhenTheGivenPostIdExists()
    {
        // ARRANGE
        var postId = Guid.NewGuid();
        var query = new GetPostByIdQuery(postId);
        var handler = new GetPostByIdQueryHandler(_postsRepository);

        var expectedPost = new Post()
        {
            Id = postId,
            Title = "Hello, World",
            Content = "Hello, from the actual world!",
            Status = PostStatus.Published
        };

        _postsRepository.GetPostByIdAsync(postId, Arg.Any<CancellationToken>())
            .Returns(expectedPost);

        // ACT
        var result = await handler.Handle(query, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        await _postsRepository.Received(1).GetPostByIdAsync(postId, Arg.Any<CancellationToken>());
    }
}