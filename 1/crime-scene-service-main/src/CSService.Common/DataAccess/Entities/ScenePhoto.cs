namespace CSService.Common.DataAccess.Entities;

/// <summary>
/// Фотография сцены места преступления.
/// </summary>
public sealed class ScenePhoto : EntityBase
{
    /// <summary>
    /// ID сцены.
    /// </summary>
    public long SceneId { get; set; }

    /// <summary>
    /// Название файла фотографии.
    /// </summary>
    public string Filename { get; set; }

    /// <summary>
    /// Порядковый номер фотографии в сцене (для сортировки).
    /// </summary>
    public int OrderIndex { get; set; }
}
