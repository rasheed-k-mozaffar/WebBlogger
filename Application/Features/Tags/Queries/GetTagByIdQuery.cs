using Domain.Models;
using MediatR;

namespace Application.Features.Tags.Queries;

public class GetTagByIdQuery(Guid id) : IRequest<Tag>
{
    public Guid Id { get; } = id;
}