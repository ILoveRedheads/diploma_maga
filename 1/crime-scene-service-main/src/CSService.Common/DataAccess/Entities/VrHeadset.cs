namespace CSService.Common.DataAccess.Entities;

/// <summary>
/// Сущность VR гарнитуры.
/// </summary>
public sealed class VrHeadset : EntityBase
{
    /// <summary>
    /// Название VR гарнитуры.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// IP адресс VR гарнитуры.
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// MAC адрес VR гарниутры.
    /// </summary>
    public string MacAddress { get; set; }

    /// <summary>
    /// Уникальный идентификатор сцены места преступления.
    /// </summary>
    public long? SceneId { get; set; }
}
