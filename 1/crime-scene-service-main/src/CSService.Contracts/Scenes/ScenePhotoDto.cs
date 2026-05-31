namespace CSService.Contracts.Scenes;

/// <summary>
/// DTO для фотографии сцены.
/// </summary>
public sealed class ScenePhotoDto
{
    /// <summary>
    /// ID фотографии.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// ID сцены.
    /// </summary>
    public long SceneId { get; set; }

    /// <summary>
    /// Порядковый номер.
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Ссылка на полноразмерное фото.
    /// </summary>
    public string PhotoLink { get; set; }

    /// <summary>
    /// Ссылка на превью.
    /// </summary>
    public string PreviewLink { get; set; }
}
