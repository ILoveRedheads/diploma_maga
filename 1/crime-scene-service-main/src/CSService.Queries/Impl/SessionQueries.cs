using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CSService.Common.Authorization;
using CSService.Common.Buffers;
using CSService.Common.DataAccess;
using CSService.Common.DataAccess.Entities;
using CSService.Common.Exceptions;
using CSService.Common.FileAccess;
using CSService.Common.Helpers;
using CSService.Common.Services;
using CSService.Contracts;
using CSService.Contracts.Comments;
using CSService.Contracts.Photos;
using CSService.Contracts.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace CSService.Queries.Impl;

internal sealed partial class SessionQueries(
    ISqlRepository repository,
    IBuffer buffer,
    IVrHeadsetContext vrHeadsetContext,
    IFileStorage fileStorage,
    IDocxService docxService) : ISessionQueries
{
    public async Task<long> CreateAsync(SessionSetDto dto) {
        var sceneId = await repository.Query<long?>(CHECK_SCENE_SQL, new { Id = vrHeadsetContext.SceneId })
            ?? throw new NotFoundException($"Сцена с ID {vrHeadsetContext.SceneId} не найдена!");

        var session = new Session {
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            GroupName = dto.GroupName,
            SceneId = sceneId,
            VrHeadsetId = vrHeadsetContext.Id
        };

        return await repository.Query<long>(CREATE_SQL, session);
    }

    public async Task CreateCommentAsync(long id, IFormFile audioFile) {
        var _ = await repository.Query<long?>(CHECK_SESSION_SQL, new { Id = id })
            ?? throw new NotFoundException($"Сессия с ID {id} не найдена!");

        using (var audioStream = audioFile.OpenReadStream()) {
            await fileStorage.Create(audioStream, ContentType.Audio, audioFile.FileName);
        }

        var comment = new Comment {
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            SessionId = id,
            AudioFilename = audioFile.FileName,
            TextFilename = string.Empty
        };

        comment.Id = await repository.Query<long>(CREATE_COMMENT_SQL, comment);
        buffer.Push(comment);
    }

    public async Task CreatePhotoAsync(long id, IFormFile screenshotFile, IFormFile audioFile) {
        var _ = await repository.Query<long?>(CHECK_SESSION_SQL, new { Id = id })
            ?? throw new NotFoundException($"Сессия с ID {id} не найдена!");

        using (var screenshotStream = screenshotFile.OpenReadStream()) {
            await fileStorage.Create(screenshotStream, ContentType.Screenshot, screenshotFile.FileName);
        }

        using (var audioStream = audioFile.OpenReadStream()) {
            await fileStorage.Create(audioStream, ContentType.Audio, audioFile.FileName);
        }

        var photo = new Photo {
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            SessionId = id,
            ScreenshotFilename = screenshotFile.FileName,
            AudioFilename = audioFile.FileName,
            TextFilename = string.Empty
        };

        photo.Id = await repository.Query<long>(CREATE_PHOTO_SQL, photo);
        buffer.Push(photo);
    }

    public async Task DeleteAsync(long id) {
        var @params = new { Id = id };
        var photos = await repository.QueryList<Photo>(GET_PHOTOS_SQL, @params);
        var comments = await repository.QueryList<Comment>(GET_COMMENTS_SQL, @params);

        foreach (var photo in photos) {
            fileStorage.Delete(ContentType.Screenshot, photo.ScreenshotFilename);
            fileStorage.Delete(ContentType.Audio, photo.AudioFilename);
            fileStorage.Delete(ContentType.Text, photo.TextFilename);
        }

        foreach (var comment in comments) {
            fileStorage.Delete(ContentType.Audio, comment.AudioFilename);
            fileStorage.Delete(ContentType.Text, comment.TextFilename);
        }

        await repository.Execute(DELETE_SQL, new { Id = id });
    }

    public async Task<SessionDto> GetAsync(long id) {
        var @params = new { Id = id };
        var session = await repository.Query<SessionDto>(GET_BY_ID_SQL, @params)
            ?? throw new NotFoundException($"Сессия с ID {id} не найдена!");

        var photos = await repository.QueryList<Photo>(GET_PHOTOS_SQL, @params);
        var comments = await repository.QueryList<Comment>(GET_COMMENTS_SQL, @params);

        session.Photos = photos.Select(photo => new PhotoDto {
            Text = fileStorage.GetTextFromFile(photo.TextFilename),
            AudioLink = string.Format(
                "api/sessions/photos/{0}/audio?hash={1}",
                photo.Id,
                HashHelper.ComputeHash(photo.AudioFilename)),
            ScreenshotLink = string.Format(
                "api/sessions/photos/{0}/screenshot?hash={1}",
                photo.Id,
                HashHelper.ComputeHash(photo.ScreenshotFilename))
        }).ToArray();

        session.Comments = comments.Select(comment => new CommentDto {
            Text = fileStorage.GetTextFromFile(comment.TextFilename),
            AudioLink = string.Format(
                "api/sessions/comments/{0}/audio?hash={1}",
                comment.Id,
                HashHelper.ComputeHash(comment.AudioFilename))
        }).ToArray();

        return session;
    }

    public async Task<MediaResult> GetCommentAudioAsync(MediaContext context) {
        var comment = await repository.Query<Comment>(GET_COMMENT_SQL, context)
            ?? throw new NotFoundException($"Комментарий с ID {context.Id} не найден!");

        var hash = HashHelper.ComputeHash(comment.AudioFilename);
        if (!string.Equals(hash, context.Hash)) throw new ArgumentException("Неверная ссылка на файл!");

        return new MediaResult {
            Content = fileStorage.GetContentStream(ContentType.Audio, comment.AudioFilename),
            ContentType = "audio/" + Path.GetExtension(comment.AudioFilename),
            Name = comment.AudioFilename
        };
    }

    public async Task<PageResult<SessionShortDto>> GetPageAsync(PageQuery query) {
        var sql = GET_PAGE_SQL;
        var countSql = COUNT_SQL;
        object @params = new { query.Limit, Offset = query.Limit * query.Page };

        if (!string.IsNullOrEmpty(query.Search)) {
            sql = GET_PAGE_WHERE_SQL;
            countSql = COUNT_WHERE_SQL;
            @params = new { Search = $"%{query.Search}%", query.Limit, Offset = query.Limit * query.Page };
        }

        var totalCount = await repository.Query<int>(countSql, @params);
        var sessions = await repository.QueryList<Session, Scene, VrHeadset, SessionShortDto>(
            sql,
            (session, scene, vrHeadset) => new SessionShortDto {
                Id = session.Id,
                FirstName = session.FirstName,
                LastName = session.LastName,
                GroupName = session.GroupName,
                SceneName = scene.Name,
                VrHeadsetName = vrHeadset.Name,
            }, @params);

        return new PageResult<SessionShortDto> {
            Page = query.Page,
            Count = sessions.Count(),
            TotalCount = totalCount,
            Data = sessions
        };
    }

    public async Task<MediaResult> GetPhotoAudioAsync(MediaContext context) {
        var photo = await repository.Query<Photo>(GET_PHOTO_SQL, context)
            ?? throw new NotFoundException($"Фото комментарий с ID {context.Id} не найден!");

        var hash = HashHelper.ComputeHash(photo.AudioFilename);
        if (!string.Equals(hash, context.Hash)) throw new ArgumentException("Неверная ссылка на файл!");

        return new MediaResult {
            Content = fileStorage.GetContentStream(ContentType.Audio, photo.AudioFilename),
            ContentType = "audio/" + Path.GetExtension(photo.AudioFilename),
            Name = photo.AudioFilename
        };
    }

    public async Task<MediaResult> GetPhotoScreenshotAsync(MediaContext context) {
        var photo = await repository.Query<Photo>(GET_PHOTO_SQL, context)
            ?? throw new NotFoundException($"Фото комментарий с ID {context.Id} не найден!");

        var hash = HashHelper.ComputeHash(photo.ScreenshotFilename);
        if (!string.Equals(hash, context.Hash)) throw new ArgumentException("Неверная ссылка на файл!");

        return new MediaResult {
            Content = fileStorage.GetContentStream(ContentType.Screenshot, photo.ScreenshotFilename),
            ContentType = "image/" + Path.GetExtension(photo.ScreenshotFilename),
            Name = photo.ScreenshotFilename
        };
    }

    public async Task<MediaResult> GetProtocolAsync(long id) {
        var @params = new { Id = id };
        _ = await repository.Query<SessionDto>(GET_BY_ID_SQL, @params)
            ?? throw new NotFoundException($"Сессия с ID {id} не найдена!");

        var photos = await repository.QueryList<Photo>(GET_PHOTOS_SQL, @params);
        var comments = await repository.QueryList<Comment>(GET_COMMENTS_SQL, @params);

        if (comments.IsNullOrEmpty())
            throw new ArgumentException("Отсутсвуют комментарии для формирования протокола");

        return new MediaResult {
            Content = docxService.CreateReports(photos, comments.First()),
            ContentType = "application/zip",
            Name = "reports.zip"
        };
    }
}
