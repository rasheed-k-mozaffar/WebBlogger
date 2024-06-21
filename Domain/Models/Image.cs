namespace Domain.Models;

public class Image
{
    public Guid Id { get; set; }

    public required string Url { get; set; }

    public required string AbsolutePath { get; set; }

    // These will be used for managing images later
    // if an image doesn't belong somewhere, it'll be shown that it's obsolete
    // and can be deleted by the admin
    public Guid? PostId { get; set; }

    public Post? Post { get; set; }

    public Guid? TagId { get; set; }

    public Tag? Tag { get; set; }
}