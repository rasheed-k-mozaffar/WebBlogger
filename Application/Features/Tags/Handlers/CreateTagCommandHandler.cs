using Application.Common.Interfaces;
using Application.Common.Mappers;
using Application.Features.Tags.Commands;
using Domain.Models;
using FluentValidation;
using MediatR;

namespace Application.Features.Tags.Handlers;

public class CreateTagCommandHandler(ITagsRepository tagsRepository, IValidator<Tag> validator)
    : IRequestHandler<CreateTagCommand, Tag>
{
    public async Task<Tag> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = request.MapToTag();
        var validationResult = await validator.ValidateAsync(tag, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        await tagsRepository.SaveTagAsync(tag, cancellationToken);
        return tag;
    }
}