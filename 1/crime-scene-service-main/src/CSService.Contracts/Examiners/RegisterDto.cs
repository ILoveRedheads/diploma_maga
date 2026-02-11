using System.ComponentModel.DataAnnotations;

namespace CSService.Contracts.Examiners;

public sealed record RegisterDto
{
    [StringLength(128)]
    public required string FirstName { get; set; }

    [StringLength(128)]
    public required string LastName { get; set; }

    [StringLength(128)]
    public required string Login { get; set; }

    [StringLength(128)]
    public required string Password { get; set; }
}
