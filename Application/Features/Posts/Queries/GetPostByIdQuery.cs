using Domain.Models;
using MediatR;

namespace Application.Features.Posts.Queries;

public record GetPostByIdQuery(Guid PostId) : IRequest<Post?>;