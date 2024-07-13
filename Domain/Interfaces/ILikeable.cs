using Domain.Models;

namespace Domain.Interfaces;

public interface ILikeable
{
    public ICollection<Like> Likes { get; set; }

    public uint LikeCount { get; set; }
}