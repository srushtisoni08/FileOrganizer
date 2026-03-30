using FileOrganizer.Models;
using FileOrganizer.Services;

// ── Parse CLI arguments ────────────────────────────────────────────────────

if (args.Length < 2)
{
    PrintUsage();
    return 1;
}

var source = args[0];
var destination = args[1];
var copy = HasFlag("--copy");
var recursive = !HasFlag("--no-recursive");
var dryRun = HasFlag("--dry-run");
var onDuplicate = GetDuplicateStrategy();

bool HasFlag(string flag) =>
    args.Any(a => a.Equals(flag, StringComparison.OrdinalIgnoreCase));

DuplicateStrategy GetDuplicateStrategy()
{
    if (HasFlag("--overwrite")) return DuplicateStrategy.Overwrite;
    if (HasFlag("--skip")) return DuplicateStrategy.Skip;
    return DuplicateStrategy.Rename;
}

// ── Build config ───────────────────────────────────────────────────────────

var config = new OrganizerConfig
{
    SourceDirectory = source,
    DestinationDirectory = destination,
    CopyInsteadOfMove = copy,
    Recursive = recursive,
    DryRun = dryRun,
    OnDuplicate = onDuplicate
};

// ── Run ────────────────────────────────────────────────────────────────────

Console.WriteLine("File Organizer");
Console.WriteLine("==============");
Console.WriteLine($"Source      : {config.SourceDirectory}");
Console.WriteLine($"Destination : {config.DestinationDirectory}");
Console.WriteLine($"Mode        : {(config.CopyInsteadOfMove ? "Copy" : "Move")}");
Console.WriteLine($"Recursive   : {config.Recursive}");
Console.WriteLine($"Dry Run     : {config.DryRun}");
Console.WriteLine($"Duplicates  : {config.OnDuplicate}");
Console.WriteLine();

try
{
    var service = new FileOrganizerService(config);
    var progress = new Progress<string>(msg => Console.WriteLine(msg));
    var result = service.Organize(config, progress);

    Console.WriteLine();
    Console.WriteLine("── Summary ──────────────────────────────────────");
    Console.WriteLine(result);

    if (result.HasErrors)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Errors:");
        result.Errors.ForEach(e => Console.WriteLine($"  • {e}"));
        Console.ResetColor();
        return 2;
    }

    return 0;
}
catch (DirectoryNotFoundException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[Error] {ex.Message}");
    Console.ResetColor();
    return 3;
}

// ── Helpers ────────────────────────────────────────────────────────────────

static void PrintUsage()
{
    Console.WriteLine("""
        Usage:
          FileOrganizer <source> <destination> [options]

        Options:
          --copy           Copy files instead of moving them
          --no-recursive   Scan only the top-level source folder
          --dry-run        Preview changes without touching files
          --overwrite      Overwrite duplicate files at destination
          --skip           Skip duplicate files (default: rename with _1, _2 …)

        Example:
          FileOrganizer C:\Downloads C:\Organized --copy --dry-run
        """);
}
