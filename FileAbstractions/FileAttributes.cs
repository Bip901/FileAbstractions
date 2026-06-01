using System;
using System.IO;

namespace FileAbstractions;

public record class FileAttributes
{
    /// <summary>
    /// The size of the file in bytes.
    /// </summary>
    public ulong? FileSize { get; init; }

    /// <summary>
    /// Whether this is a directory (true) or a regular file (false).
    /// </summary>
    public bool IsDirectory { get; init; }

    /// <summary>
    /// The last file write time, UTC.
    /// </summary>
    public DateTimeOffset? LastModifiedTime { get; init; }

    /// <summary>
    /// The last file access time, UTC.
    /// </summary>
    public DateTimeOffset? LastAccessedTime { get; init; }

    public static FileAttributes FromFileSystemInfo(FileSystemInfo fileSystemInfo)
    {
        ulong? fileSize = null;
        bool isDirectory = true;
        if (fileSystemInfo is FileInfo fileInfo)
        {
            fileSize = (ulong?)fileInfo.Length;
            isDirectory = false;
        }
        return new FileAttributes()
        {
            FileSize = fileSize,
            IsDirectory = isDirectory,
            LastModifiedTime = fileSystemInfo.LastWriteTimeUtc,
            LastAccessedTime = fileSystemInfo.LastAccessTimeUtc,
        };
    }
}
