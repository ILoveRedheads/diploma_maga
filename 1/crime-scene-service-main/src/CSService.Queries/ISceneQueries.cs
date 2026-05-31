using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CSService.Contracts;
using CSService.Contracts.Scenes;

namespace CSService.Queries;

public interface ISceneQueries
{
    Task<long> CreateAsync(string name, string fileName, Stream stream);

    Task<long> AddPhotoToSceneAsync(long sceneId, string fileName, Stream stream);

    Task<PageResult<SceneDto>> GetPageAsync(PageQuery context);

    Task<MediaResult> GetPreviewAsync(MediaContext context);

    Task<MediaResult> GetAsync();

    Task<IEnumerable<ScenePhotoDto>> GetScenePhotosAsync(long sceneId);

    Task<MediaResult> GetScenePhotoAsync(long photoId);

    Task<MediaResult> GetScenePhotoByIndexAsync(int photoIndex);

    Task<SceneMetaDto> GetSceneMetaAsync();
}
