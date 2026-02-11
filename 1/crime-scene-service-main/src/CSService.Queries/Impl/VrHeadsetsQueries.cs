using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSService.Common.DataAccess;
using CSService.Common.DataAccess.Entities;
using CSService.Common.Helpers;
using CSService.Contracts.VrHeadsets;

namespace CSService.Queries.Impl;

internal sealed partial class VrHeadsetsQueries(ISqlRepository sqlRepository) : IVrHeadsetQueries
{
    public async Task CreateAsync(VrHeadsetSetDto setDto) {
        var vrHeadset = await sqlRepository.Query<VrHeadset>(GET_SQL, new { setDto.Name, setDto.MacAddress });
        if (vrHeadset is not null) {
            throw new ArgumentException("VR гарнитура с таким именем или MAC адресом уже существует!");
        }

        vrHeadset = new VrHeadset {
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            Name = setDto.Name,
            MacAddress = setDto.MacAddress
        };

        await sqlRepository.Execute(CREATE_SQL, vrHeadset);
    }

    public Task<IEnumerable<VrHeadsetDto>> GetListAsync() =>
        sqlRepository.QueryList<VrHeadsetDto, Scene, VrHeadsetDto>(
            GET_ALL_SQL,
            (vrHeadset, scene) => new VrHeadsetDto {
                Id = vrHeadset.Id,
                Ip = vrHeadset.Ip,
                Name = vrHeadset.Name,
                MacAddress = vrHeadset.MacAddress,
                SceneId = scene?.Id,
                SceneName = scene?.Name,
                ScenePreviewLink = scene is not null
                    ? string.Format("api/scenes/{0}/preview?hash={1}", scene.Id, HashHelper.ComputeHash(scene.Filename))
                    : null
            });

    public async Task UpdateAsync(long id, VrHeadsetSetDto setDto) {
        var vrHeadset = await sqlRepository.Query<VrHeadset>(GET_BY_ID_SQL, new { id })
            ?? throw new ArgumentException($"VR гарнитура с ID {id} не найдена!");

        var sameNameVrHeadset = await sqlRepository.Query<VrHeadset>(GET_SAME_SQL, new { Id = id, setDto.Name, setDto.MacAddress });
        if (sameNameVrHeadset is not null) {
            throw new ArgumentException("VR гарнитура с таким именем или MAC адресом уже существует!");
        }

        vrHeadset.UpdateDate = DateTime.UtcNow;
        vrHeadset.Name = setDto.Name;
        vrHeadset.MacAddress = setDto.MacAddress;
        vrHeadset.SceneId = setDto.SceneId;

        await sqlRepository.Execute(UPDATE_SQL, vrHeadset);
    }
}
