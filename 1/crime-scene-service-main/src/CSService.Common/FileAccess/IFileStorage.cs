using System.IO;
using System.Threading.Tasks;

namespace CSService.Common.FileAccess;

public interface IFileStorage
{
    Task Create(Stream content, ContentType contentType, string name);

    Task CreateTextFile(string text, string name);

    FileStream GetContentStream(ContentType contentType, string name);

    string GetTextFromFile(string name);

    string GetPath(ContentType contentType, string filename);

    void Delete(ContentType contentType, string filename);
}
