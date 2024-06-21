namespace Domain.Models;

public class BookmarkedPost
{
    public Guid Id { get; set; }

    public Guid AppUserId { get; set; }

    public AppUser AppUser { get; set; } = new();

    public Guid PostId { get; set; }

    public Post Post { get; set; } = new();
}