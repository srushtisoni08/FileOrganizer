using FileOrganizer.Models;

namespace FileOrganizer.Services;

public class FileOrganizerService
{
    private readonly FileScanner _scanner;
    private readonly FileClassifier _classifier;
    private readonly FileMover _mover;

    public FileOrganizerService(OrganizerConfig config)
    {
        _scanner = new FileScanner();
        _classifier = new FileClassifier(config.ExtensionMap);
        _mover = new FileMover();
    }

    /// <summary>
    /// Scans, classifies, and transfers files according to <paramref name="config"/>.
    /// Reports progress via <paramref name="progress"/>.
    /// </summary>
    public OrganizeResult Organize(OrganizerConfig config, IProgress<string>? progress = null)
    {
        var result = new OrganizeResult();

        // 1. Scan
        progress?.Report($"Scanning: {config.SourceDirectory}");
        var entries = _scanner.Scan(config.SourceDirectory, config.Recursive).ToList();
        result.TotalScanned = entries.Count;
        progress?.Report($"Found {entries.Count} file(s).");

        // 2. Classify
        _classifier.Classify(entries, config.DestinationDirectory);

        // 3. Transfer
        foreach (var entry in entries)
        {
            var error = _mover.Transfer(entry, config.CopyInsteadOfMove, config.DryRun, config.OnDuplicate);

            if (error is not null)
            {
                result.Errors.Add(error);
                progress?.Report($"  [ERROR]   {error}");
            }
            else if (entry.DestinationPath is null)
            {
                result.Skipped++;
                progress?.Report($"  [SKIP]    {entry.FileName}");
            }
            else
            {
                result.Moved++;
                var verb = config.DryRun ? "(dry-run)" : config.CopyInsteadOfMove ? "Copied" : "Moved";
                progress?.Report($"  [{verb}] {entry}");
            }
        }

        return result;
    }
}