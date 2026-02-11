using System;

namespace CSService.Common.Services.Models;

public record CommentModel
{
    public string FirstName { get; init; }

    public string LastName { get; init; }

    public string GroupName { get; init; }

    public string SceneName { get; init; }

    public DateTime CreateDate { get; init; }

    public string AudioFilename { get; init; }

    public string PhotoFilename { get; init; }

    public string TextFilename { get; init; }
}
