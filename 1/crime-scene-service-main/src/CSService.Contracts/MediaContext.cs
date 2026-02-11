namespace CSService.Contracts;

public sealed record MediaContext
{
    public long Id { get; set; }

    public string Hash { get; set; }
}
