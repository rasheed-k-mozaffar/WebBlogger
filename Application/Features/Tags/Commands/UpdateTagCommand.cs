using Domain.Models;
using MediatR;

namespace Application.Features.Tags.Commands;

public record UpdateTagCommand
(
    Guid Id,
    string Name,
    string? Description,
    string? CoverImageUrl
) : IRequest<Tag>;