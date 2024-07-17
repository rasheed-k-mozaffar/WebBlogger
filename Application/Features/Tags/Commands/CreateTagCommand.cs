using Domain.Models;
using MediatR;

namespace Application.Features.Tags.Commands;

public record CreateTagCommand
(
    Guid Id,
    string Name,
    string? Description,
    string? CoverImageUrl
): IRequest<Tag>;