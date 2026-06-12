using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions.Implementations;

/// <summary>
/// An implementation of <see cref="IVirtualFileOrDirectory"/> backed by the operating system's filesystem.
/// </summary>
/// <param name="localPath">The local path of this item.</param>
public abstract class LocalFileOrDirectory(string localPath) : IMovable, IVirtualFileOrDirectory
{
    /// <summary>
    /// The file name (including the extension but excluding the path) of this file or directory.
    /// </summary>
    public string Name => Path.GetFileName(LocalPath);

    /// <summary>
    /// The full local path of this item.
    /// </summary>
    public string LocalPath { get; private set; } = localPath;

    /// <inheritdoc/>
    public Task RenameAsync(string newName, bool allowOverwrite, CancellationToken cancellationToken)
    {
        SanitizeName(newName);
        string newLocalPath = Path.Join(Path.GetDirectoryName(LocalPath), newName);
        MoveTo(newLocalPath, allowOverwrite);
        LocalPath = newLocalPath;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task MoveToAsync(
        IVirtualDirectory newParent,
        string newName,
        bool allowOverwrite,
        CancellationToken cancellationToken
    )
    {
        SanitizeName(newName);
        if (newParent is not LocalDirectory newParentDir)
        {
            throw new InvalidOperationException($"Can't move \"{LocalPath}\" under non-local directory.");
        }
        string newLocalPath = Path.Join(newParentDir.LocalPath, newName);
        MoveTo(newLocalPath, allowOverwrite);
        LocalPath = newLocalPath;
        return Task.CompletedTask;
    }

    /// <summary>
    /// A non-async variant of <see cref="MoveToAsync"/>, called internally after sanitzation.
    /// </summary>
    protected abstract void MoveTo(string newPath, bool allowOverwrite);

    /// <inheritdoc/>
    public Task<FileAttributes> GetAttributesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(FileAttributes.FromFileSystemInfo(GetFileSystemInfo()));
    }

    /// <inheritdoc/>
    public Task SetAttributesAsync(FileAttributes attributes, CancellationToken cancellationToken)
    {
        SetAttributes(GetFileSystemInfo(), attributes);
        return Task.CompletedTask;
    }

    /// <summary>
    /// A non-async variant of <see cref="SetAttributesAsync"/>, called internally.
    /// </summary>
    protected static void SetAttributes(FileSystemInfo fileSystemInfo, FileAttributes attributes)
    {
        // When adding supported attributes here, don't forget to check them in LocalFile.OpenAsync.
        if (attributes.FileSize != null && fileSystemInfo is FileInfo fileInfo)
        {
            using FileStream stream = fileInfo.Open(FileMode.Open, FileAccess.Write);
            stream.SetLength((long)attributes.FileSize);
        }
        if (attributes.LastAccessedTime != null && attributes.LastAccessedTime != DateTimeOffset.MinValue)
        {
            fileSystemInfo.LastAccessTimeUtc = attributes.LastAccessedTime.Value.UtcDateTime;
        }
        if (attributes.LastModifiedTime != null && attributes.LastModifiedTime != DateTimeOffset.MinValue)
        {
            fileSystemInfo.LastWriteTimeUtc = attributes.LastModifiedTime.Value.UtcDateTime;
        }
    }

    /// <summary>
    /// When overriden by a child class, returns the local file system info of this item.
    /// </summary>
    protected abstract FileSystemInfo GetFileSystemInfo();

    /// <inheritdoc/>
    public abstract Task DeleteAsync(CancellationToken cancellationToken);

    /// <exception cref="ArgumentException">The name was invalid.</exception>
    public static void SanitizeName(ReadOnlySpan<char> name)
    {
        if (name.Contains(Path.DirectorySeparatorChar) || name.Contains(Path.AltDirectorySeparatorChar))
        {
            throw new ArgumentException("File name must not include a directory separator.", nameof(name));
        }
        if (name.IsEmpty)
        {
            throw new ArgumentException("Name cannot be empty", nameof(name));
        }
        if (name == "." || name == "..")
        {
            throw new ArgumentException($"Special directory '{nameof(name)}' is not supported", nameof(name));
        }
    }
}
