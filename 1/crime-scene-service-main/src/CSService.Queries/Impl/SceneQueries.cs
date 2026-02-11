using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CSService.Common.Authorization;
using CSService.Common.DataAccess;
using CSService.Common.Exceptions;
using CSService.Common.FileAccess;
using CSService.Common.Helpers;
using CSService.Common.Services;
using CSService.Contracts;
using CSService.Contracts.Scenes;
using CSService.Common.DataAccess.Entities;

namespace CSService.Queries.Impl;

internal sealed partial class SceneQueries(
    ISqlRepository sqlRepository,
    IFileStorage fileStorage,
    IPhotoCompressionService photoCompressiosService,
    IVrHeadsetContext vrHeadsetContext) : ISceneQueries
{
    public async Task<long> CreateAsync(string name, string fileName, Stream stream) {
        var scene = await sqlRepository.Query<Scene>(GET_BY_NAME_SQL, new { name });
        if (scene is not null) {
            throw new ArgumentException("Сцена с таким именем уже существует!");
        }

        await fileStorage.Create(stream, ContentType.Scene, fileName);
        stream.Seek(0, SeekOrigin.Begin);

        await fileStorage.Create(
            await photoCompressiosService.Compress(stream, 4),
            ContentType.CompressedPhoto,
            fileName);

        scene = new Scene {
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Name = name,
            Filename = fileName
        };

        return await sqlRepository.Query<long>(CREATE_SQL, scene);
    }

    public async Task<MediaResult> GetAsync() {
        var scene = await sqlRepository.Query<Scene>(GET_BY_ID_SQL, new { Id = vrHeadsetContext.SceneId })
            ?? throw new NotFoundException("Не удалось найти такую сцену!");

        return new MediaResult {
            Content = fileStorage.GetContentStream(ContentType.Scene, scene.Filename),
            ContentType = "image/" + Path.GetExtension(scene.Filename),
            Name = scene.Name
        };
    }

    public async Task<PageResult<SceneDto>> GetPageAsync(PageQuery context) {
        var scenes = (await sqlRepository.QueryList<Scene>(
                GET_PAGE_SQL,
                new { context.Limit, Offset = context.Limit * context.Page }))
            .Select(scene => new SceneDto {
                Id = scene.Id,
                Name = scene.Name,
                PreviewLink = string.Format("api/scenes/{0}/preview?hash={1}", scene.Id, HashHelper.ComputeHash(scene.Filename))
            });

        var totalCount = await sqlRepository.Query<int>(COUNT_SQL);

        return new PageResult<SceneDto> {
            Page = context.Page,
            Count = scenes.Count(),
            TotalCount = totalCount,
            Data = scenes
        };
    }

    public async Task<MediaResult> GetPreviewAsync(MediaContext context) {
        var scene = await sqlRepository.Query<Scene>(GET_BY_ID_SQL, new { context.Id })
            ?? throw new NotFoundException($"Не удалось найти такую сцену");

        var hash = HashHelper.ComputeHash(scene.Filename);
        if (!string.Equals(hash, context.Hash)) throw new ArgumentException("Неверная ссылка на файл!");

        return new MediaResult {
            Content = fileStorage.GetContentStream(ContentType.CompressedPhoto, scene.Filename),
            ContentType = "image/" + Path.GetExtension(scene.Filename),
            Name = scene.Name
        };
    }
}
