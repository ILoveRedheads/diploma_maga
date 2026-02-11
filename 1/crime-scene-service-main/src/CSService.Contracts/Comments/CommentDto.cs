namespace CSService.Contracts.Comments;

public sealed record CommentDto
{
    public string AudioLink { get; init; }

    public string Text { get; init; }
}
