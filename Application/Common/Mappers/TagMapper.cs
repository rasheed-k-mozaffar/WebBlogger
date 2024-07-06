using Application.Features.Tags.Commands;
using Domain.Models;

namespace Application.Common.Mappers;

public static class TagMapper
{
    public static Tag MapToTag(this CreateTagCommand command)
        => new Tag()
        {
            Id = command.Id,
            Name = command.Name,
            Description = command.Description,
            CoverImageUrl = command.CoverImageUrl
        };

    public static Tag MapToTag()
    {
        //TODO: Write the mapper to map from UpdateTagCommand
        return new Tag()
        {
            Id = Guid.NewGuid(),
            Name = string.Empty
        };
    }
}