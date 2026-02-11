using System.ComponentModel.DataAnnotations;

namespace CSService.Contracts.VrHeadsets;

public record VrHeadsetSetDto
{
    [StringLength(64)]
    public required string Name { get; set; }

    [StringLength(17)]
    public required string MacAddress { get; set; }

    public long? SceneId { get; set; }
}
