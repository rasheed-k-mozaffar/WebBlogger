namespace Domain.Models;

public class Tag
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public ICollection<Post>? Posts { get; set; }

    public Guid? CoverImageId { get; set; }

    public Image? CoverImage { get; set; }
}