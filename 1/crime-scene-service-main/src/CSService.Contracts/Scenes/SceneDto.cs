using System.Collections.Generic;

namespace CSService.Contracts.Scenes;

public sealed record SceneDto
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string PreviewLink { get; set; }

    /// <summary>
    /// Количество фотографий в сцене.
    /// </summary>
    public int PhotoCount { get; set; }
}
