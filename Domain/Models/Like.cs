namespace Domain.Models;

public class Like
{
    public Guid Id { get; set; }

    public Guid AppUserId { get; set; }

    public AppUser AppUser { get; set; } = null!;

    public Guid? PostId { get; set; }

    public Post? Post { get; set; }

    public Guid? CommentId { get; set; }

    public Comment? Comment { get; set; }

    public DateTime Timestamp { get; set; }
}