using CSService.Common.DataAccess.Entities;

namespace CSService.Common.Buffers;

public interface IBuffer
{
    void Push(ISessionEntity entity);
}
