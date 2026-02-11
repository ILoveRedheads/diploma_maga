using System;
using System.Buffers;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Vosk;

namespace CSService.Common.Services.Impl;

internal sealed class VoiceRecognitionService(IVoiceRegonitionSettings settings) : IVoiceRecognitionService
{
    private Model _model = null;

    public Task<string> GetTextFromAudio(WaveStream waveStream) {
        _model ??= new Model(settings.ModelPath);

        var recognizer = new VoskRecognizer(_model, 16_000f);
        recognizer.SetMaxAlternatives(0);
        recognizer.SetWords(true);

        using var resampleStream = new MemoryStream();
        var resampler = new WdlResamplingSampleProvider(waveStream.ToSampleProvider(), 16_000);
        WaveFileWriter.WriteWavFileToStream(resampleStream, resampler.ToWaveProvider16());

        resampleStream.Seek(0, SeekOrigin.Begin);

        var buffer = ArrayPool<byte>.Shared.Rent(4096);
        int bytesRead;

        while ((bytesRead = resampleStream.Read(buffer, 0, buffer.Length)) > 0) {
            recognizer.AcceptWaveform(buffer, bytesRead);
        }

        var result = recognizer.FinalResult().AsSpan();
        var startIndex = result.IndexOf("text") + 9;

        ArrayPool<byte>.Shared.Return(buffer, clearArray: true);

        return Task.FromResult(result[startIndex..^3].ToString());
    }
}
