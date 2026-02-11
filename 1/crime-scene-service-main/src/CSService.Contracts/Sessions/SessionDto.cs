using CSService.Contracts.Comments;
using CSService.Contracts.Photos;

namespace CSService.Contracts.Sessions;

public sealed record SessionDto : SessionShortDto
{
    public PhotoDto[] Photos { get; set; }

    public CommentDto[] Comments { get; set; }
}
