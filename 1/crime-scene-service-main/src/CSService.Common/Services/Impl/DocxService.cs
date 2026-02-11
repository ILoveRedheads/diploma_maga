using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CSService.Common.FileAccess;
using CSService.Common.DataAccess.Entities;
using NPOI.XWPF.UserModel;
using Wordroller;

namespace CSService.Common.Services.Impl;

internal sealed class DocxService : IDocxService
{
    private const string REPORT_FODLER = "./reports";
    private const string TEMPLATE_PATH = "./templates/Протокол осмотра места происшествия.docx";
    private const string PROTOCOL_PATH = "./reports/Протокол осмотра места происшествия.docx";
    private const string PHOTO_PROTOCOL_PATH = "./reports/Фотографии сделанные во время осмотра места происшествия.docx";

    private readonly IFileStorage _fileStorage;

    public DocxService(IFileStorage fileStorage) {
        _fileStorage = fileStorage;
        if (!Directory.Exists(REPORT_FODLER)) Directory.CreateDirectory(REPORT_FODLER);
    }

    public Stream CreateReports(IEnumerable<Photo> photos, Comment comment) {
        // Photo protocol
        using var photoReport = new XWPFDocument();
        var p1 = photoReport.CreateParagraph();
        p1.Alignment = ParagraphAlignment.CENTER;
        p1.SpacingBetween = 1.5f;

        var r1 = p1.CreateRun();
        r1.FontSize = 14;
        r1.SetFontFamily("Times New Roman", FontCharRange.Ascii);
        r1.SetText($"Фотографии сделанные во время осмотра места происшествия");

        var number = 1;
        foreach (var photo in photos) {
            var p2 = photoReport.CreateParagraph();
            p2.Alignment = ParagraphAlignment.CENTER;
            p2.SpacingBetween = 1.5f;

            var r2 = p2.CreateRun();
            using var picture = _fileStorage.GetContentStream(ContentType.Screenshot, photo.ScreenshotFilename);
            r2.AddPicture(picture, (int)PictureType.PNG, $"Рисунок {number}", 480 * 9525, 270 * 9525);
            r2.AddBreak(BreakType.TEXTWRAPPING);
            r2.FontSize = 14;
            r2.SetFontFamily("Times New Roman", FontCharRange.Ascii);
            r2.AppendText($"Рисунок {number} - {_fileStorage.GetTextFromFile(photo.TextFilename)}");

            number++;
        }

        using (var photosFileStream = new FileStream(PHOTO_PROTOCOL_PATH, FileMode.OpenOrCreate)) {
            photoReport.Write(photosFileStream);
        }

        // Main protocol
        using var templateStream = new FileStream(TEMPLATE_PATH, FileMode.Open, System.IO.FileAccess.Read);
        using var template = new WordDocument(templateStream);
        var text = _fileStorage.GetTextFromFile(comment.TextFilename).AsSpan();

        var index = Math.Min(36, text.Length);

        template.Body
            .FindText("{{short-text}}", StringComparison.InvariantCulture)
            .Single()
            .ReplaceWithTextRun(text[..index].ToString(), true);

        var tags = template.Body.FindText("{{text}}", StringComparison.InvariantCulture);
        foreach (var tag in tags) {
            if (index == text.Length) {
                tag.ReplaceWithTextRun(string.Empty, true);
                continue;
            }

            // Search for the end of a word
            var endIndex = Math.Min(index + 73, text.Length);

            for (; endIndex < text.Length && text[endIndex] != ' '; endIndex++) { }

            tag.ReplaceWithTextRun(text[index..endIndex].ToString(), true);
            index = endIndex;
        }

        using (var protocolFileStream = new FileStream(
            PROTOCOL_PATH,
            FileMode.Create,
            System.IO.FileAccess.Write,
            FileShare.Write)) {
            template.Save(protocolFileStream);
        }

        var memoryStream = new MemoryStream();
        ZipFile.CreateFromDirectory(REPORT_FODLER, memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        Directory.Delete(REPORT_FODLER, recursive: true);

        return memoryStream;
    }
}
