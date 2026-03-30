using FileOrganizer.Models;

namespace FileOrganizer.Services;

public class FileMover
{
    /// <summary>
    /// Moves (or copies) <paramref name="entry"/> to its <see cref="FileEntry.DestinationPath"/>.
    /// Returns an error message on failure, or null on success.
    /// </summary>
    public string? Transfer(FileEntry entry, bool copy, bool dryRun, DuplicateStrategy onDuplicate)
    {
        try
        {
            var destination = ResolveDestination(entry.DestinationPath, onDuplicate);

            if (destination is null)
                return null; // Skipped intentionally

            // Ensure the category sub-folder exists
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

            if (!dryRun)
            {
                if (copy)
                    File.Copy(entry.FullPath, destination, overwrite: onDuplicate == DuplicateStrategy.Overwrite);
                else
                    File.Move(entry.FullPath, destination, overwrite: onDuplicate == DuplicateStrategy.Overwrite);
            }

            // Update the entry so callers can see the resolved path
            entry.DestinationPath = destination;
            return null;
        }
        catch (Exception ex)
        {
            return $"{entry.FileName}: {ex.Message}";
        }
    }

    // ------------------------------------------------------------------ //
    //  Duplicate resolution                                                //
    // ------------------------------------------------------------------ //

    /// <summary>
    /// Returns the final destination path after applying the duplicate strategy.
    /// Returns null if the strategy is Skip and the file exists.
    /// </summary>
    private static string? ResolveDestination(string requested, DuplicateStrategy strategy)
    {
        if (!File.Exists(requested))
            return requested;

        return strategy switch
        {
            DuplicateStrategy.Overwrite => requested,
            DuplicateStrategy.Skip => null,
            DuplicateStrategy.Rename => BuildUniqueName(requested),
            _ => requested
        };
    }

    private static string BuildUniqueName(string path)
    {
        var dir = Path.GetDirectoryName(path)!;
        var nameNoExt = Path.GetFileNameWithoutExtension(path);
        var ext = Path.GetExtension(path);
        var counter = 1;

        string candidate;
        do
        {
            candidate = Path.Combine(dir, $"{nameNoExt}_{counter++}{ext}");
        }
        while (File.Exists(candidate));

        return candidate;
    }
}