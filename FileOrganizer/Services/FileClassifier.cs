using FileOrganizer.Models;

namespace FileOrganizer.Services;

public class FileClassifier
{
    private readonly Dictionary<string, string> _map;

    public FileClassifier(Dictionary<string, string> extensionMap)
    {
        _map = extensionMap;
    }

    /// <summary>
    /// Sets <see cref="FileEntry.Category"/> and <see cref="FileEntry.DestinationPath"/>
    /// on each entry in-place.
    /// </summary>
    public void Classify(IEnumerable<FileEntry> entries, string destinationRoot)
    {
        foreach (var entry in entries)
        {
            entry.Category = ResolveCategory(entry.Extension);
            entry.DestinationPath = BuildDestination(entry, destinationRoot);
        }
    }

    private string ResolveCategory(string extension) =>
        _map.TryGetValue(extension, out var cat) ? cat : "Uncategorized";

    private static string BuildDestination(FileEntry entry, string destinationRoot) =>
        Path.Combine(destinationRoot, entry.Category, entry.FileName);
}