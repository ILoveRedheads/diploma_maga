namespace CSService.Contracts.Sessions;

public record SessionShortDto : SessionSetDto
{
    public long Id { get; set; }

    public string SceneName { get; init; }

    public string VrHeadsetName { get; init; }
}
