using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Models;

public class Post : ILikeable
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public Guid? CoverImageId { get; set; }

    public Image? CoverImage { get; set; }

    public DateTime PublishedOn { get; set; }

    public DateTime? LastEditedOn { get; set; }

    public PostStatus Status { get; set; }

    public ICollection<Tag>? Tags { get; set; } = [];

    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<BookmarkedPost> BookmarkedPosts { get; set; } = [];

    public uint Views { get; set; }

    public ICollection<Like> Likes { get; set; } = [];

    public uint LikeCount { get; set; }
}