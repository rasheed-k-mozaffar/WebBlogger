using Application.Common.Interfaces;
using Application.Features.Tags.Queries;
using Domain.Exceptions;
using Domain.Models;
using MediatR;

namespace Application.Features.Tags.Handlers;

public class GetTagByIdQueryHandler(ITagsRepository tagsRepository) : IRequestHandler<GetTagByIdQuery, Tag>
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="request">A <see cref="GetTagByIdQuery"/> request containing the tag id</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to compelte</param>
    /// <exception cref="TagNotFoundException">If no tag was found with the given id, a <see cref="TagNotFoundException"/>
    /// is thrown from the tags repository</exception>
    /// <returns></returns>
    public async Task<Tag> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await tagsRepository.GetTagByIdAsync(request.Id, cancellationToken);
    }
}