using System.Reflection.PortableExecutable;

namespace Domain.Models;

public class Comment
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime WrittenOn { get; set; }

    public DateTime? LastEditedOn { get; set; }

    public Guid AuthorId { get; set; }

    public AppUser Author { get; set; } = new AppUser();

    public Guid PostId { get; set; }

    public Post Post { get; set; }

    public Guid? ParentCommentId { get; set; }

    public Comment? ParentComment { get; set; }

    public ICollection<Comment>? Replies { get; set; } = [];

    public ICollection<Like> Likes { get; set; } = [];

    public uint LikeCount { get; set; }
}