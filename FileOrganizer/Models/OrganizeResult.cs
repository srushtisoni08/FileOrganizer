namespace FileOrganizer.Models;

public class OrganizeResult
{
    public int TotalScanned { get; set; }
    public int Moved { get; set; }
    public int Skipped { get; set; }
    public List<string> Errors { get; } = new();

    public bool HasErrors => Errors.Count > 0;

    public override string ToString() =>
        $"Scanned: {TotalScanned} | Moved: {Moved} | Skipped: {Skipped} | Errors: {Errors.Count}";
}