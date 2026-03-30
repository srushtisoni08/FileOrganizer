using FileOrganizer.Models;

namespace FileOrganizer.Services;

public class FileScanner
{
    /// <summary>
    /// Enumerates all files in <paramref name="directory"/>.
    /// Hidden files and system files are excluded.
    /// </summary>
    public IEnumerable<FileEntry> Scan(string directory, bool recursive)
    {
        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException($"Source directory not found: {directory}");

        var searchOption = recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        return Directory
            .EnumerateFiles(directory, "*.*", searchOption)
            .Select(path => new FileInfo(path))
            .Where(fi => !IsHiddenOrSystem(fi))
            .Select(FileEntry.FromFileInfo);
    }

    private static bool IsHiddenOrSystem(FileInfo fi) =>
        fi.Attributes.HasFlag(FileAttributes.Hidden) ||
        fi.Attributes.HasFlag(FileAttributes.System);
}