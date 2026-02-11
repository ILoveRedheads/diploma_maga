namespace CSService.Domain;

/// <summary>
/// Сущность записи о преподователе.
/// </summary>
public sealed class Examiner : EntityBase
{
    /// <summary>
    /// Имя.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Логин учетной записи.
    /// </summary>
    public string Login { get; set; }

    /// <summary>
    /// Хешированный пароль.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Соль от хеша пароля.
    /// </summary>
    public string PasswordSalt { get; set; }
}
