using System.Collections.Generic;

namespace CSService.Contracts;

public sealed record PageResult<TData>
{
    public int Page { get; set; }

    public int Count { get; set; }

    public int TotalCount { get; set; }

    public IEnumerable<TData> Data { get; set; }
}
