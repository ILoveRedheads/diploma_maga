namespace CSService.Contracts.VrHeadsets;

public sealed record VrHeadsetDto : VrHeadsetSetDto
{
    public long Id { get; set; }

    public string Ip { get; set; }

    public string SceneName { get; set; }

    public string ScenePreviewLink { get; set; }
}
