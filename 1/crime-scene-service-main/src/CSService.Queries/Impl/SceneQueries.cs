using System;
using System.Collections.Generic;
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

        var sceneId = await sqlRepository.Query<long>(CREATE_SQL, scene);

        // Создаём первую фотографию для сцены
        var scenePhoto = new ScenePhoto {
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            SceneId = sceneId,
            Filename = fileName,
            OrderIndex = 0
        };

        await sqlRepository.Execute(CREATE_SCENE_PHOTO_SQL, scenePhoto);

        return sceneId;
    }

    public async Task<long> AddPhotoToSceneAsync(long sceneId, string fileName, Stream stream) {
        var scene = await sqlRepository.Query<Scene>(GET_BY_ID_SQL, new { Id = sceneId })
            ?? throw new NotFoundException("Сцена не найдена!");

        await fileStorage.Create(stream, ContentType.Scene, fileName);
        stream.Seek(0, SeekOrigin.Begin);

        await fileStorage.Create(
            await photoCompressiosService.Compress(stream, 4),
            ContentType.CompressedPhoto,
            fileName);

        var maxOrderIndex = await sqlRepository.Query<int>(GET_MAX_ORDER_INDEX_SQL, new { SceneId = sceneId });

        var scenePhoto = new ScenePhoto {
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            SceneId = sceneId,
            Filename = fileName,
            OrderIndex = maxOrderIndex + 1
        };

        return await sqlRepository.Query<long>(CREATE_SCENE_PHOTO_SQL, scenePhoto);
    }

    public async Task<MediaResult> GetAsync() {
        // Получаем первое фото сцены (с индексом 0)
        return await GetScenePhotoByIndexAsync(0);
    }

    public async Task<IEnumerable<ScenePhotoDto>> GetScenePhotosAsync(long sceneId) {
        var photos = await sqlRepository.QueryList<ScenePhoto>(GET_SCENE_PHOTOS_SQL, new { SceneId = sceneId });

        return photos.Select(photo => new ScenePhotoDto {
            Id = photo.Id,
            SceneId = photo.SceneId,
            OrderIndex = photo.OrderIndex,
            PhotoLink = string.Format("api/scenes/photos/{0}?hash={1}", photo.Id, HashHelper.ComputeHash(photo.Filename)),
            PreviewLink = string.Format("api/scenes/photos/{0}/preview?hash={1}", photo.Id, HashHelper.ComputeHash(photo.Filename))
        });
    }

    public async Task<MediaResult> GetScenePhotoAsync(long photoId) {
        var photo = await sqlRepository.Query<ScenePhoto>(GET_SCENE_PHOTO_BY_ID_SQL, new { Id = photoId })
            ?? throw new NotFoundException("Фотография не найдена!");

        return new MediaResult {
            Content = fileStorage.GetContentStream(ContentType.Scene, photo.Filename),
            ContentType = "image/" + Path.GetExtension(photo.Filename),
            Name = photo.Filename
        };
    }

    public async Task<SceneMetaDto> GetSceneMetaAsync() {
        var meta = await sqlRepository.Query<SceneMetaDto>(
            GET_SCENE_META_SQL,
            new { MacAddress = vrHeadsetContext.MacAddress })
            ?? throw new NotFoundException("Для данной VR-гарнитуры не назначена сцена!");

        return meta;
    }

    public async Task<MediaResult> GetScenePhotoByIndexAsync(int photoIndex) {
        var photo = await sqlRepository.Query<ScenePhoto>(
            GET_SCENE_PHOTO_BY_INDEX_SQL,
            new { MacAddress = vrHeadsetContext.MacAddress, OrderIndex = photoIndex })
            ?? throw new NotFoundException($"Фотография с индексом {photoIndex} не найдена!");

        return new MediaResult {
            Content = fileStorage.GetContentStream(ContentType.Scene, photo.Filename),
            ContentType = "image/" + Path.GetExtension(photo.Filename),
            Name = photo.Filename
        };
    }

    public async Task<PageResult<SceneDto>> GetPageAsync(PageQuery context) {
        var scenes = await sqlRepository.QueryList<Scene>(
            GET_PAGE_SQL,
            new { context.Limit, Offset = context.Limit * context.Page });

        var sceneDtos = new List<SceneDto>();
        
        foreach (var scene in scenes) {
            var photoCount = await sqlRepository.Query<int>(COUNT_SCENE_PHOTOS_SQL, new { SceneId = scene.Id });
            
            sceneDtos.Add(new SceneDto {
                Id = scene.Id,
                Name = scene.Name,
                PreviewLink = string.Format("api/scenes/{0}/preview?hash={1}", scene.Id, HashHelper.ComputeHash(scene.Filename)),
                PhotoCount = photoCount
            });
        }

        var totalCount = await sqlRepository.Query<int>(COUNT_SQL);

        return new PageResult<SceneDto> {
            Page = context.Page,
            Count = sceneDtos.Count,
            TotalCount = totalCount,
            Data = sceneDtos
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
