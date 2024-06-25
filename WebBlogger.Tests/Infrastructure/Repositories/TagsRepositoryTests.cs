using Infrastructure.Data;
using WebBlogger.Tests.Shared;
using Application.Common.Options;
using Infrastructure.Repositories;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Domain.Exceptions;
using Application.Common.Requests;
using Application.Common.Enums;

namespace WebBlogger.Tests.Infrastructure.Repositories;

public class TagsRepositoryTests
{
    private const string InMemDatabaseName = "in-mem-db";

    [Fact]
    public async Task SaveTag_ShouldReturnTheCreatedTag()
    {
        // ARRANGE
        await using var context = new AppDbContext(
            DatabaseContextOptionsBuilder.CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());

        var tag = new Tag()
        {
            Id = Guid.NewGuid(),
            Name = "Test Tag",
            Description = "Dummy description",
            CoverImageUrl = null
        };

        var tagsRepository = new TagsRepository(context);

        // ACT
        var savedTag = await tagsRepository.SaveTagAsync(tag, CancellationToken.None);

        // ASSERT
        savedTag.Should().NotBeNull();
        savedTag.Id.Should().Be(tag.Id);
    }

    [Fact]
    public async Task GetTags_ShouldReturnAllTags_AsReadOnlyCollection()
    {
        // ARRANGE
        int expectedNumberOfTags = 5;

        await using var context = new AppDbContext(
            DatabaseContextOptionsBuilder.CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());

        var tagsRepository = new TagsRepository(context);

        InMemoryDatabaseSeeder.SeedTags(context, expectedNumberOfTags);

        //ACT
        var result = await tagsRepository.GetTagsAsync(CancellationToken.None);

        // ASSERT
        result.Should().NotBeEmpty();
        result.Should().BeOfType<ReadOnlyCollection<Tag>>();
        result.Should().HaveCount(expectedNumberOfTags);
    }

    [Fact]
    public async Task DeleteTag_ShouldReturnTrue_IfTagWasDeletedCorrectly()
    {
        // ARRANGE
        await using var context = new AppDbContext(
            DatabaseContextOptionsBuilder.CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());

        var tagsRepository = new TagsRepository(context);

        InMemoryDatabaseSeeder.SeedTags(context, 1);

        var tagToDelete = await context.Tags.FirstAsync();

        // ACT
        bool result = await tagsRepository.DeleteTagAsync(tagToDelete.Id, CancellationToken.None);

        // ASSERT
        var deletedPost = await context.Tags.FirstOrDefaultAsync(t => t.Id == tagToDelete.Id);
        result.Should().Be(true);
        deletedPost.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTag_ShouldReturnTheUpdatedTag()
    {
        // ARRANGE
        await using var context = new AppDbContext(
            DatabaseContextOptionsBuilder.CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());

        var tagsRepository = new TagsRepository(context);

        InMemoryDatabaseSeeder.SeedTags(context, 1);

        var tagToUpdate = await context.Tags.FirstAsync();
        var updatedTag = new Tag()
        {
            Name = "Updated Tag",
            Description = "Updated Description"
        };

        // ACT
        var result = await tagsRepository
        .UpdateTagAsync(tagToUpdate.Id, updatedTag, CancellationToken.None);

        // ASSERT
        result.Should().NotBeNull();
        result.Name.Should().Be(updatedTag.Name);
    }

    [Fact]
    public async Task GetTagById_ShouldThrowTagNotFoundException_WhenTagDoesNotExist()
    {
        // ARRANGE
        await using var context = new AppDbContext(
            DatabaseContextOptionsBuilder.CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());

        var tagsRepository = new TagsRepository(context);
        var randomId = Guid.NewGuid();

        // ACT
        var action = async () => await tagsRepository.GetTagByIdAsync(randomId, CancellationToken.None);

        // ASSERT  
        await action.Should().ThrowAsync<TagNotFoundException>();
    }

    [Fact]
    public async Task GetPostsByTag_ShouldReturnPaginatedReadOnlyPostsCollection_WhenTagHasPostsAssociatedWithIt()
    {
        // ARRANGE
        int expectedNumberOfPosts = 10;
        int expectedTotal = 20;
        await using var context = new AppDbContext(
            DatabaseContextOptionsBuilder.CreateNewContextOptions(InMemDatabaseName), new EntityFrameworkOptions());

        var tagsRepository = new TagsRepository(context);

        InMemoryDatabaseSeeder.SeedTagPosts(context, expectedTotal);
        var tag = await context.Tags.FirstAsync();
        var paginationRequest = new PaginationRequest<PostSortOption>(1, 10, PostSortOption.MostRecent, null);

        // ACT
        var result = await tagsRepository.GetPostsByTagAsync(tag.Id, paginationRequest, CancellationToken.None);

        //ASSERT
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(expectedNumberOfPosts);
        result.Items.Should().AllSatisfy(x => x.Tags?.Contains(tag));
        result.TotalCount.Should().Be(expectedTotal);
    }
}