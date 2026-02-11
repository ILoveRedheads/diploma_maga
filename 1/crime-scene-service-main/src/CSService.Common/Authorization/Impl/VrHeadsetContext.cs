using System;
using CSService.Common.DataAccess.Entities;

namespace CSService.Common.Authorization.Impl;

internal sealed class VrHeadsetContext : IVrHeadsetContext
{
    public string MacAddress => _vrHeadset.MacAddress;

    public long Id => _vrHeadset.Id;

    public string Name => _vrHeadset.Name;

    public long? SceneId => _vrHeadset.SceneId;

    private VrHeadset _vrHeadset;

    public void SetCurrentVrHeadset(VrHeadset vrHeadset) {
        if (_vrHeadset is not null) {
            throw new ArgumentException("VR headset context already set!");
        }

        _vrHeadset = vrHeadset;
    }
}
