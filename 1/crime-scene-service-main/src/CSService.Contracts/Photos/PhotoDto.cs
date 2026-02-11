namespace CSService.Contracts.Photos;

public sealed record PhotoDto
{
    public string ScreenshotLink { get; init; }

    public string AudioLink { get; init; }

    public string Text { get; init; }
}
