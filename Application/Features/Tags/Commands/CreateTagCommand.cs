using Domain.Models;
using MediatR;

namespace Application.Features.Tags.Commands;

public class CreateTagCommand : IRequest<Tag>
{
    public Guid Id { get; init; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? CoverImageUrl { get; set; }
}