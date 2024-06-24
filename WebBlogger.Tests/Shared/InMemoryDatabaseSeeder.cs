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
}