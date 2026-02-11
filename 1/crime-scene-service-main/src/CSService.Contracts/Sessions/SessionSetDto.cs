namespace CSService.Contracts.Sessions;

public record SessionSetDto
{
    public string FirstName { get; init; }

    public string LastName { get; init; }

    public string GroupName { get; init; }
}
