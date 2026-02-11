using System;
using System.IO;
using System.Threading.Tasks;

namespace CSService.Common.FileAccess.Impl;

internal sealed class FileStorage : IFileStorage
{
    private const string SCREENSHOT_FOLDER = "./screenshots";
    private const string COMPRESSED_PHOTO_FOLDER = "./compressed_photos";
    private const string AUDIO_FOLDER = "./audios";
    private const string SCENE_FOLDER = "./scenes";
    private const string TEXT_FOLDER = "./texts";

    public FileStorage() {
        if (!Directory.Exists(SCREENSHOT_FOLDER)) Directory.CreateDirectory(SCREENSHOT_FOLDER);
        if (!Directory.Exists(COMPRESSED_PHOTO_FOLDER)) Directory.CreateDirectory(COMPRESSED_PHOTO_FOLDER);
        if (!Directory.Exists(SCENE_FOLDER)) Directory.CreateDirectory(SCENE_FOLDER);
        if (!Directory.Exists(AUDIO_FOLDER)) Directory.CreateDirectory(AUDIO_FOLDER);
        if (!Directory.Exists(TEXT_FOLDER)) Directory.CreateDirectory(TEXT_FOLDER);
    }

    public async Task Create(Stream content, ContentType contentType, string name) {
        var pathToFile = GetPath(contentType, name);
        if (File.Exists(pathToFile)) {
            var fileExtenstion = Path.GetExtension(pathToFile);
            pathToFile = pathToFile.Insert(pathToFile.Length - fileExtenstion.Length - 1, DateTime.UtcNow.ToString("dd_MM_yyyy_hh_mm_ss"));
        }

        using (var fileStream = new FileStream(pathToFile, FileMode.Create)) {
            content.Seek(0, SeekOrigin.Begin);
            await content.CopyToAsync(fileStream);
        }
    }

    public FileStream GetContentStream(ContentType contentType, string name) {
        var pathToFile = GetPath(contentType, name);
        if (!File.Exists(pathToFile)) throw new ArgumentException("File not exists!");

        return new FileStream(pathToFile, FileMode.Open, System.IO.FileAccess.Read, FileShare.Read);
    }

    public Task CreateTextFile(string text, string name) {
        var pathToFile = GetPath(ContentType.Text, name);
        if (File.Exists(pathToFile)) {
            var fileExtenstion = Path.GetExtension(pathToFile);
            pathToFile = pathToFile.Insert(pathToFile.Length - fileExtenstion.Length - 1, DateTime.UtcNow.ToString("dd_MM_yyyy_hh_mm_ss"));
        }

        return File.WriteAllTextAsync(pathToFile, text);
    }

    public string GetTextFromFile(string name) {
        var pathToFile = GetPath(ContentType.Text, name);
        if (!File.Exists(pathToFile)) return string.Empty;

        return File.ReadAllText(pathToFile);
    }

    public void Delete(ContentType contentType, string filename) {
        var path = GetPath(contentType, filename);
        if (File.Exists(path)) File.Delete(path);
    }

    public string GetPath(ContentType contentType, string name) => contentType switch {
        ContentType.Screenshot => Path.Combine(SCREENSHOT_FOLDER, name),
        ContentType.CompressedPhoto => Path.Combine(COMPRESSED_PHOTO_FOLDER, name),
        ContentType.Scene => Path.Combine(SCENE_FOLDER, name),
        ContentType.Audio => Path.Combine(AUDIO_FOLDER, name),
        ContentType.Text => Path.Combine(TEXT_FOLDER, name),
        _ => throw new ArgumentException("Incorrect content type!")
    };
}
