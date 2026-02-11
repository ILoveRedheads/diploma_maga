namespace CSService.Common.DataAccess.Entities;

public sealed class Session : EntityBase
{
    /// <summary>
    /// Уникальный идентификатор сцены.
    /// </summary>
    public long SceneId { get; set; }

    /// <summary>
    /// Уникальный идентификатор VR гарнитуры. 
    /// </summary>
    public long VrHeadsetId { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Название группы пользователя.
    /// </summary>
    public string GroupName { get; set; }
}
