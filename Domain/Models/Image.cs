namespace Domain.Models;

public class Image
{
    public Guid Id { get; set; }

    public required string Url { get; set; }

    public required string AbsolutePath { get; set; }
}