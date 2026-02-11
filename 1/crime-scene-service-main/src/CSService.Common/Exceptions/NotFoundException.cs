using System.Net;

namespace CSService.Common.Exceptions;

public sealed class NotFoundException : BaseException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public NotFoundException(string message) : base(message) { }
}
