using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class BookmarkedPostConfiguration : IEntityTypeConfiguration<BookmarkedPost>
{
    public void Configure(EntityTypeBuilder<BookmarkedPost> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.AppUserId, x.PostId })
            .IsUnique();

        builder.HasOne(x => x.AppUser)
            .WithMany(x => x.BookmarkedPosts)
            .HasForeignKey(x => x.AppUserId);

        builder.HasOne(x => x.Post)
            .WithMany(x => x.BookmarkedPosts)
            .HasForeignKey(x => x.PostId);
    }
}