using FileOrganizer.Models;
using FileOrganizer.Services;

namespace FileOrganizer.Tests;

public class FileClassifierTests
{
    private static FileClassifier BuildClassifier() =>
        new(OrganizerConfig.DefaultExtensionMap());

    [Theory]
    [InlineData("photo.jpg", "Images")]
    [InlineData("clip.mp4", "Videos")]
    [InlineData("song.mp3", "Audio")]
    [InlineData("report.pdf", "Documents")]
    [InlineData("app.exe", "Executables")]
    [InlineData("archive.zip", "Archives")]
    [InlineData("main.cs", "Code")]
    [InlineData("unknown.xyz", "Uncategorized")]
    public void Classify_AssignsCorrectCategory(string fileName, string expectedCategory)
    {
        var entry = new FileEntry
        {
            FullPath = $"/source/{fileName}",
            FileName = fileName,
            Extension = Path.GetExtension(fileName).TrimStart('.').ToLower()
        };

        var classifier = BuildClassifier();
        classifier.Classify(new[] { entry }, "/dest");

        Assert.Equal(expectedCategory, entry.Category);
    }

    [Fact]
    public void Classify_SetsDestinationPath()
    {
        var entry = new FileEntry
        {
            FullPath = "/src/image.png",
            FileName = "image.png",
            Extension = "png"
        };

        BuildClassifier().Classify(new[] { entry }, "/dest");

        Assert.Equal(Path.Combine("/dest", "Images", "image.png"), entry.DestinationPath);
    }
}

public class FileScannerTests
{
    [Fact]
    public void Scan_ThrowsWhenDirectoryMissing()
    {
        var scanner = new FileScanner();
        Assert.Throws<DirectoryNotFoundException>(() =>
            scanner.Scan("/nonexistent_path_xyz", false).ToList());
    }

    [Fact]
    public void Scan_ReturnsFilesFromDirectory()
    {
        var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(dir);

        try
        {
            File.WriteAllText(Path.Combine(dir, "a.txt"), "hello");
            File.WriteAllText(Path.Combine(dir, "b.pdf"), "world");

            var scanner = new FileScanner();
            var entries = scanner.Scan(dir, false).ToList();

            Assert.Equal(2, entries.Count);
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }
}