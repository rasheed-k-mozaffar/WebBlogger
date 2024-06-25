using Application.Common.Enums;
using Application.Common.Requests;
using Application.Common.Responses;
using Domain.Models;

namespace Application.Common.Interfaces;

public interface ITagsRepository
{
    Task<Tag> SaveTagAsync(Tag tag, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Tag>> GetTagsAsync(CancellationToken cancellationToken);

    Task<bool> DeleteTagAsync(Guid tagId, CancellationToken cancellationToken);

    Task<Tag> UpdateTagAsync(Guid tagId, Tag tag, CancellationToken cancellationToken);

    Task<PaginatedReadOnlyCollection<Post>> GetPostsByTagAsync
        (Guid tagId, PaginationRequest<PostSortOption> paginationRequest, CancellationToken cancellationToken);

    Task<Tag> GetTagByIdAsync(Guid tagId, CancellationToken cancellationToken);
}