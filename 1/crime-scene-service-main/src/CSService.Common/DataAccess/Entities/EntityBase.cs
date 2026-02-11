using System;

namespace CSService.Common.DataAccess.Entities;

/// <summary>
/// Базовая класс для сущности в БД.
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Уникальный идентификатор сущности.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Дата и время создание сущности.
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// Дата и время обновления сущности.
    /// </summary>
    public DateTime UpdateDate { get; set; }
}
