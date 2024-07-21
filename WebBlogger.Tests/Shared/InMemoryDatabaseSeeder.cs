using Bogus;
using Domain.Enums;
using Infrastructure.Data;

namespace WebBlogger.Tests.Shared;

public class InMemoryDatabaseSeeder
{
    public static void SeedPosts(AppDbContext context, int numOfPosts)
    {
        var faker = new Faker<Post>()
            .RuleFor(p => p.Title, f => f.Lorem.Sentence())
            .RuleFor(p => p.Content, f => f.Lorem.Paragraphs(2))
            .RuleFor(p => p.PublishedOn, f => f.Date.Past())
            .RuleFor(p => p.Status, f => f.PickRandom<PostStatus>());

        var posts = faker.Generate(numOfPosts);
        context.Posts.AddRange(posts);
        context.SaveChanges();
    }

    public static void SeedTags(AppDbContext context, int numOfTags)
    {
        var faker = new Faker<Tag>()
            .RuleFor(p => p.Name, f => f.Lorem.Sentence(1))
            .RuleFor(p => p.Description, f => f.Lorem.Sentences(3));

        var tags = faker.Generate(numOfTags);
        context.Tags.AddRange(tags);
        context.SaveChanges();
    }

    public static void SeedTagPosts(AppDbContext context, int numOfPosts)
    {
        var tag = new Tag()
        {
            Name = "Test Tag",
            Description = "Test Tag Description",
            CoverImageUrl = null,
            Posts = []
        };

        context.Tags.Add(tag);

        var faker = new Faker<Post>()
            .RuleFor(p => p.Title, f => f.Lorem.Sentence())
            .RuleFor(p => p.Content, f => f.Lorem.Paragraphs(2))
            .RuleFor(p => p.PublishedOn, f => f.Date.Past())
            .RuleFor(p => p.Status, f => f.PickRandom<PostStatus>());

        var posts = faker.Generate(numOfPosts);

        foreach (var p in posts)
        {
            p.Tags?.Add(tag);
        }

        context.Posts.AddRange(posts);
        context.SaveChanges();
    }

    public static void SeedComments(AppDbContext context, Post post, int numOfComments)
    {
        var faker = new Faker<Comment>()
            .RuleFor(p => p.Content, f => f.Lorem.Paragraphs(2))
            .RuleFor(p => p.WrittenOn, f => f.Date.Past())
            .RuleFor(p => p.PostId, post.Id)
            .RuleFor(p => p.Post, post);

        var comments = faker.Generate(numOfComments);
        context.Comments.AddRange(comments);
        context.SaveChanges();
    }

    public static void SeedPostLikes(AppDbContext context, Post post, int numOfLikes)
    {
        var faker = new Faker<Like>()
            .RuleFor(p => p.PostId, post.Id)
            .RuleFor(p => p.Post, post)
            .RuleFor(p => p.Id, f => f.Random.Guid());

        var likes = faker.Generate(numOfLikes);
        context.Likes.AddRange(likes);
        context.SaveChanges();
    }
}