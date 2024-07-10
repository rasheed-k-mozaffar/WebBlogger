using Application.Common.Interfaces;
using Application.Features.Tags.Commands;
using MediatR;

namespace Application.Features.Tags.Handlers;

public class DeleteTagCommandHandler(ITagsRepository tagsRepository) : IRequestHandler<DeleteTagCommand, bool>
{
    /// <summary>
    /// Handle deleting the tag using the tag id provided inside the request object
    /// </summary>
    /// <param name="request">A <see cref="DeleteTagCommand"/> object containing the id of tag to delete</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to signal if operation was cancelled</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<bool> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        return await tagsRepository.DeleteTagAsync(request.Id, cancellationToken);
    }
}