using Domain.Models;
using MediatR;

namespace Application.Features.Posts.Queries;

public class GetPostByIdQuery(Guid postId) : IRequest<Post?>
{
    public Guid PostId { get; } = postId;
}