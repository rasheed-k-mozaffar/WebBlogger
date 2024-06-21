using Application.Common.Options;
using Domain.Models;
using Infrastructure.EntityConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    private readonly EntityFrameworkOptions _efOptions;


    public DbSet<Post> Posts { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<Like> Likes { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Image> Images { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, EntityFrameworkOptions efOptions) : base(options)
    {
        _efOptions = efOptions;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (_efOptions.EnableDetailedErrors)
            optionsBuilder.EnableDetailedErrors();

        if (_efOptions.EnableSensitiveLogs)
            optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new AppUserConfiguration());
        builder.ApplyConfiguration(new PostConfiguration());
        builder.ApplyConfiguration(new CommentConfiguration());
        builder.ApplyConfiguration(new TagConfiguration());
        builder.ApplyConfiguration(new ImageConfiguration());
        builder.ApplyConfiguration(new LikeConfiguration());
        builder.ApplyConfiguration(new BookmarkedPostConfiguration());
    }
}