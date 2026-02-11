using System.IO;

namespace CSService.Contracts;

public sealed record MediaResult
{
    public Stream Content { get; set; }

    public string ContentType { get; set; }

    public string Name { get; set; }
}
