using MediatR;

namespace Application.Features.Tags.Commands;

public class DeleteTagCommand(Guid id) : IRequest<bool>
{
    public Guid Id { get; } = id;
}