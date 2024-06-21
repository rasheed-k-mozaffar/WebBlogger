using Domain.Enums;
using MediatR;

namespace Application.Features.Posts.Commands;

public class DeletePostCommand : IRequest
{
    public Guid PostId { get; set; }

    public DeleteType DeleteType { get; set; } = DeleteType.Soft;
}