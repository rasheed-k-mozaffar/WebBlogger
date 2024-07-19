using Domain.Models;
using MediatR;

namespace Application.Features.Comments.Queries;

public record GetCommentByIdQuery
(
    Guid Id
) : IRequest<Comment?>;