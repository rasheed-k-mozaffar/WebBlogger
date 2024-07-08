using System.Collections.ObjectModel;
using Domain.Models;
using MediatR;

namespace Application.Features.Tags.Queries;

public class GetAllTagsQuery : IRequest<IReadOnlyCollection<Tag>>
{

}