using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CSService.Contracts;

public sealed record ErrorDetails
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("detail")]
    public string Detail { get; set; }

    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
