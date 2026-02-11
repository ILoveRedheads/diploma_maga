namespace CSService.Common.DataAccess.Entities;

public sealed class ProtocolComment : EntityBase
{
    /// <summary>
    /// Уникальный идентификатор сессии.
    /// </summary
    public long SessionId { get; set; }

    /// <summary>
    /// Имя файла с аудио комментарием.
    /// </summary
    public string AudioFilename { get; set; }

    /// <summary>
    /// Имя файла с текстовой расшифровкой аудио комментария.
    /// </summary
     public string TextFilename { get; set; }
}
