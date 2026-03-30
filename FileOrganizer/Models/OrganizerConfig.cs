namespace FileOrganizer.Models;

public class OrganizerConfig
{
    /// <summary>Directory to scan for files.</summary>
    public string SourceDirectory { get; init; } = string.Empty;

    /// <summary>Root directory where organized folders will be created.</summary>
    public string DestinationDirectory { get; init; } = string.Empty;

    /// <summary>If true, copy files instead of moving them.</summary>
    public bool CopyInsteadOfMove { get; init; } = false;

    /// <summary>If true, scan sub-folders recursively.</summary>
    public bool Recursive { get; init; } = true;

    /// <summary>If true, don't actually move/copy — just print what would happen.</summary>
    public bool DryRun { get; init; } = false;

    /// <summary>What to do if a file with the same name exists at the destination.</summary>
    public DuplicateStrategy OnDuplicate { get; init; } = DuplicateStrategy.Rename;

    /// <summary>Extension → category mapping. Extensions should be lowercase without leading dot.</summary>
    public Dictionary<string, string> ExtensionMap { get; init; } = DefaultExtensionMap();

    // ------------------------------------------------------------------ //
    //  Built-in defaults                                                   //
    // ------------------------------------------------------------------ //
    public static Dictionary<string, string> DefaultExtensionMap() => new(StringComparer.OrdinalIgnoreCase)
    {
        // Images
        { "jpg",  "Images" }, { "jpeg", "Images" }, { "png",  "Images" },
        { "gif",  "Images" }, { "bmp",  "Images" }, { "svg",  "Images" },
        { "webp", "Images" }, { "tiff", "Images" }, { "ico",  "Images" },
        { "heic", "Images" },

        // Videos
        { "mp4",  "Videos" }, { "mkv",  "Videos" }, { "avi",  "Videos" },
        { "mov",  "Videos" }, { "wmv",  "Videos" }, { "flv",  "Videos" },
        { "webm", "Videos" }, { "m4v",  "Videos" },

        // Audio
        { "mp3",  "Audio" }, { "wav",  "Audio" }, { "flac", "Audio" },
        { "aac",  "Audio" }, { "ogg",  "Audio" }, { "m4a",  "Audio" },
        { "wma",  "Audio" },

        // Documents
        { "pdf",  "Documents" }, { "doc",  "Documents" }, { "docx", "Documents" },
        { "xls",  "Documents" }, { "xlsx", "Documents" }, { "ppt",  "Documents" },
        { "pptx", "Documents" }, { "txt",  "Documents" }, { "rtf",  "Documents" },
        { "odt",  "Documents" }, { "ods",  "Documents" }, { "odp",  "Documents" },

        // Code
        { "cs",   "Code" }, { "js",  "Code" }, { "ts",  "Code" },
        { "py",   "Code" }, { "go",  "Code" }, { "rs",  "Code" },
        { "java", "Code" }, { "cpp", "Code" }, { "c",   "Code" },
        { "h",    "Code" }, { "css", "Code" }, { "html","Code" },
        { "xml",  "Code" }, { "json","Code" }, { "yaml","Code" },
        { "yml",  "Code" }, { "sh",  "Code" }, { "ps1", "Code" },
        { "sql",  "Code" },

        // Archives
        { "zip",  "Archives" }, { "rar",  "Archives" }, { "7z",  "Archives" },
        { "tar",  "Archives" }, { "gz",   "Archives" }, { "bz2", "Archives" },
        { "xz",   "Archives" }, { "iso",  "Archives" },

        // Executables
        { "exe",  "Executables" }, { "msi",  "Executables" },
        { "deb",  "Executables" }, { "rpm",  "Executables" },
        { "dmg",  "Executables" }, { "appimage", "Executables" },
    };
}

public enum DuplicateStrategy
{
    /// <summary>Append _1, _2 … to the destination filename.</summary>
    Rename,

    /// <summary>Overwrite the existing file.</summary>
    Overwrite,

    /// <summary>Leave the source file and record it in errors.</summary>
    Skip
}