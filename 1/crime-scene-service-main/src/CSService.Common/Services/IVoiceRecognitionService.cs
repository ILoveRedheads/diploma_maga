using System.Threading.Tasks;
using NAudio.Wave;

namespace CSService.Common.Services;

public interface IVoiceRecognitionService
{
    Task<string> GetTextFromAudio(WaveStream waveStream);
}
