namespace CSService.Domain;

/// <summary>
/// Сущность сцены с места преступления.
/// </summary>
public sealed class Scene : EntityBase
{
    /// <summary>
    /// Название сцены.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Название файла фотографии сцены.
    /// </summary>
    public string Filename { get; set; }
}
