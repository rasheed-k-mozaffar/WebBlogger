using Domain.Models;
using MediatR;

namespace Application.Features.Tags.Queries;

public record GetTagByIdQuery(Guid Id) : IRequest<Tag>;