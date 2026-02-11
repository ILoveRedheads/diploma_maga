namespace CSService.Common.Authorization;

public sealed class HashedPassword
{
    public string Hash { get; set; }

    public string Salt { get; set; }
}
