using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using CSService.Common.DataAccess;
using CSService.Common.FileAccess;
using CSService.Common.Services;
using CSService.Common.DataAccess.Entities;
using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace CSService.Common.Buffers.Impl;

internal sealed class Buffer : IBuffer
{
    private readonly IFileStorage _fileStorage;
    private readonly ISqlRepository _repository;
    private readonly IVoiceRecognitionService _voiceRecognitionService;
    private readonly ActionBlock<ISessionEntity> _actionBlock;
    private readonly ILogger<Buffer> _logger;

    private const string FILENAME_PATTERN = "{0}_text_session_{1}_{2}.txt";
    private const string UPDATE_SQL = "update {0}s set text_filename = @TextFilename where id = @Id";

    public Buffer(
        ISqlRepository repository,
        IVoiceRecognitionService voiceRecognitionService,
        IFileStorage fileStorage,
        ILogger<Buffer> logger)
    {
        _repository = repository;
        _voiceRecognitionService = voiceRecognitionService;
        _fileStorage = fileStorage;
        _logger = logger;
        _actionBlock = new ActionBlock<ISessionEntity>(
            ProcessEntity,
            new ExecutionDataflowBlockOptions {
                MaxDegreeOfParallelism = 1,
            });
    }

    public void Push(ISessionEntity entity) => _actionBlock.Post(entity);

    private async Task ProcessEntity(ISessionEntity entity) {
        var entityName = entity.GetType().Name.ToLower();
        var textFilename = string.Format(FILENAME_PATTERN, entityName, entity.SessionId, DateTime.UtcNow.ToString("dd_MM_yyyy_hh_mm_ss"));

        using var audioStream = _fileStorage.GetContentStream(ContentType.Audio, entity.AudioFilename);
        using WaveStream reader = Path.GetExtension(entity.AudioFilename) switch {
            ".mp3" => new Mp3FileReader(audioStream),
            _ => new WaveFileReader(audioStream)
        };

        var text = await _voiceRecognitionService.GetTextFromAudio(reader);

        _logger.LogInformation("Text has been recognized for session {SessionId}", entity.SessionId);

        entity.TextFilename = textFilename;

        await _fileStorage.CreateTextFile(text, textFilename);
        await _repository.Execute(string.Format(UPDATE_SQL, entityName), entity);
    }
}
