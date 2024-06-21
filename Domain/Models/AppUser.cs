using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<Like> Likes { get; set; } = [];

    public ICollection<BookmarkedPost> BookmarkedPosts { get; set; } = [];
}