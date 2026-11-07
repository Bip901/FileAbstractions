using System.IO;

namespace FileAbstractions;

/// <summary>
/// An abstraction of <see cref="DriveInfo"/>.
/// </summary>
public record class FileSystemAttributes
{
    /// <summary>
    /// The total size of storage space available on the file system, in bytes.
    /// </summary>
    public long TotalSize { get; init; }

    /// <summary>
    /// The total amount of free space available on the file system, in bytes.
    /// </summary>
    public long TotalFreeSpace { get; init; }

    /// <summary>
    /// The amount of available free space on the file system, in bytes.
    /// This excludes space reserved for system or root users.
    /// </summary>
    /// <remarks>This is supposed to be less than or equal to <see cref="TotalFreeSpace"/>.</remarks>
    public long AvailableFreeSpace { get; init; }

    /// <summary>
    /// Creates new <see cref="FileSystemAttributes"/> from the given <paramref name="driveInfo"/>.
    /// </summary>
    public static FileSystemAttributes FromDriveInfo(DriveInfo driveInfo)
    {
        return new FileSystemAttributes()
        {
            TotalSize = driveInfo.TotalSize,
            TotalFreeSpace = driveInfo.TotalFreeSpace,
            AvailableFreeSpace = driveInfo.AvailableFreeSpace,
        };
    }
}
