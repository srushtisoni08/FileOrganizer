namespace FileOrganizer.Models;

public class FileEntry
{
    public string FullPath { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string Extension { get; init; } = string.Empty;
    public string Category { get; set; } = "Uncategorized";
    public string DestinationPath { get; set; } = string.Empty;
    public long SizeBytes { get; init; }
    public DateTime LastModified { get; init; }

    public static FileEntry FromFileInfo(FileInfo fi) => new()
    {
        FullPath = fi.FullName,
        FileName = fi.Name,
        Extension = fi.Extension.TrimStart('.').ToLowerInvariant(),
        SizeBytes = fi.Length,
        LastModified = fi.LastWriteTime
    };

    public override string ToString() =>
        $"{FileName} ({Category}) → {DestinationPath}";
}