namespace CSService.Contracts;

public record PageQuery
{
    public int Page { get; set; } = 0;

    public int Limit { get; set; } = 25;

    public string Search { get; set; }
}
