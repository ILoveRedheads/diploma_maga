using System.IO;
using System.Threading.Tasks;

namespace CSService.Common.Services;

public interface IPhotoCompressionService
{
    Task<MemoryStream> Compress(Stream stream, int aspectRatio);
}
