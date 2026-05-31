namespace CSService.Contracts.Scenes;

/// <summary>
/// Метаданные сцены, назначенной VR-гарнитуре.
/// </summary>
public sealed record SceneMetaDto
{
    public long SceneId { get; init; }

    public string SceneName { get; init; }

    public int PhotoCount { get; init; }
}
