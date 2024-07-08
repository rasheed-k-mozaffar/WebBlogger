using System.Collections.ObjectModel;
using Application.Common.Interfaces;
using Application.Features.Tags.Queries;
using Domain.Models;
using MediatR;

namespace Application.Features.Tags.Handlers;

public class GetAllTagsQueryHandler(ITagsRepository tagsRepository)
    : IRequestHandler<GetAllTagsQuery, IReadOnlyCollection<Tag>>
{
    public async Task<IReadOnlyCollection<Tag>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await tagsRepository
            .GetTagsAsync(cancellationToken);

        return tags;
    }
}