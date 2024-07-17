using System.Collections.ObjectModel;
using Domain.Models;
using MediatR;

namespace Application.Features.Tags.Queries;

public record GetAllTagsQuery : IRequest<IReadOnlyCollection<Tag>>;