using CSService.Common.DataAccess.Entities;

namespace CSService.Common.Authorization;

public interface IVrHeadsetContext
{
    long Id { get; }

    string MacAddress { get; }

    string Name { get; }

    long? SceneId { get; }

    void SetCurrentVrHeadset(VrHeadset vrHeadset);
}
