namespace CSService.Contracts.Scenes;

public sealed record SceneDto
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string PreviewLink { get; set; }
}
