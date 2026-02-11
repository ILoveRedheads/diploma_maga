namespace CSService.Domain;

public sealed class Photo : EntityBase, ISessionEntity
{
    /// <summary>
    /// Уникальный идентификатор  сессии.
    /// </summary>
    public long SessionId { get; set; }

    /// <summary>
    /// Имя файла с аудио комментарием.
    /// </summary>
    public string AudioFilename { get; set; }

    /// <summary>
    /// Имя файла с фотографией с места преступления.
    /// </summary>
    public string ScreenshotFilename { get; set; }

    /// <summary>
    /// Имя файла с текстовой расшифровкой аудио комментария.
    /// </summary>
    public string TextFilename { get; set; }
}
