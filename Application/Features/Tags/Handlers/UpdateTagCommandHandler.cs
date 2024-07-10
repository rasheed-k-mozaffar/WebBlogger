using Application.Common.Interfaces;
using Application.Common.Mappers;
using Application.Features.Tags.Commands;
using Domain.Exceptions;
using Domain.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Tags.Handlers;

public class UpdateTagCommandHandler(ITagsRepository tagsRepository, IValidator<Tag> validator)
    : IRequestHandler<UpdateTagCommand, Tag>
{
    /// <summary>
    /// Handles updating the tag information provided in the request
    /// </summary>
    /// <param name="request">The <see cref="UpdateTagCommand"/> command carrying the new tag information</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> object to signal cancellation of the operation</param>
    /// <exception cref="ArgumentNullException">Thrown if the <see cref="UpdateTagCommand"/> is null</exception>/// <returns></returns>
    /// <exception cref="TagNotFoundException">Thrown if the given tag id doesn't exist</exception>
    public async Task<Tag> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var updatedTag = request.MapToTag();
        var validationResult = await validator.ValidateAsync(updatedTag, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await tagsRepository.UpdateTagAsync(updatedTag.Id, updatedTag, cancellationToken);
        return updatedTag;
    }
}