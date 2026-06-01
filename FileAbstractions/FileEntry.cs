using System.IO;

namespace FileAbstractions;

public record class FileEntry(string Name, FileAttributes Attributes)
{
    public bool IsDirectory => Attributes.IsDirectory;

    public static FileEntry FromFileSystemInfo(FileSystemInfo fileSystemInfo)
    {
        return new(fileSystemInfo.Name, FileAttributes.FromFileSystemInfo(fileSystemInfo));
    }
}
