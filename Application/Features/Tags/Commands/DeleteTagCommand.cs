using MediatR;

namespace Application.Features.Tags.Commands;

public record DeleteTagCommand(Guid Id) : IRequest<bool>;