namespace CSService.Contracts.Examiners;

public sealed record LoginDto
{
    public string Login { get; set; }

    public string Password { get; set; }
}
