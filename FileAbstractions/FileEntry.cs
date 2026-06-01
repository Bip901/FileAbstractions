using System.IO;

namespace FileAbstractions;

/// <summary>
/// A tuple of a file name and <paramref name="Attributes"/>.
/// </summary>
/// <param name="Name">The name (including any extension, but not the full path) of the file or directory.</param>
/// <param name="Attributes">The file attributes.</param>
public record class FileEntry(string Name, FileAttributes Attributes)
{
    /// <summary>
    /// Whether this is a directory (true) or a regular file (false).
    /// </summary>
    public bool IsDirectory => Attributes.IsDirectory;

    /// <summary>
    /// Creates a new <see cref="FileEntry"/> from the given <paramref name="fileSystemInfo"/>.
    /// </summary>
    public static FileEntry FromFileSystemInfo(FileSystemInfo fileSystemInfo)
    {
        return new(fileSystemInfo.Name, FileAttributes.FromFileSystemInfo(fileSystemInfo));
    }
}
