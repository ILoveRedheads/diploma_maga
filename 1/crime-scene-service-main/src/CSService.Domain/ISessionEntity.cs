namespace CSService.Domain;

public interface ISessionEntity
{
    public long Id { get; set; }

    public long SessionId { get; set; }

    public string AudioFilename { get; set; }

    public string TextFilename { get; set; }
}
