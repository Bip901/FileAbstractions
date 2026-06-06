using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions.Implementations;

/// <summary>
/// An implementation of <see cref="IVirtualFile"/> backed by the operating system's filesystem.
/// </summary>
public class LocalFile(string localPath)
    : LocalFileOrDirectory(localPath),
        IReadableVirtualFile,
        IReadableWriteable,
        INamedSizedReadable
{
    /// <inheritdoc/>
    public ulong Size => (ulong)new FileInfo(LocalPath).Length;

    /// <inheritdoc/>
    protected override void MoveTo(string newPath, bool allowOverwrite)
    {
        File.Move(LocalPath, newPath, allowOverwrite);
    }

    /// <inheritdoc/>
    public override Task DeleteAsync(CancellationToken cancellationToken)
    {
        File.Delete(LocalPath);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override FileSystemInfo GetFileSystemInfo()
    {
        return new FileInfo(LocalPath);
    }

    /// <inheritdoc/>
    public Task<Stream> OpenReadAsync(FileMode fileMode, CancellationToken cancellationToken)
    {
        return OpenAsync(fileMode, FileAccess.Read, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<Stream> OpenWriteAsync(FileMode fileMode, CancellationToken cancellationToken)
    {
        return OpenAsync(fileMode, FileAccess.Write, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<Stream> OpenReadWriteAsync(FileMode fileMode, CancellationToken cancellationToken)
    {
        return OpenAsync(fileMode, FileAccess.ReadWrite, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<Stream> OpenAsync(FileMode fileMode, FileAccess fileAccess, CancellationToken cancellationToken)
    {
        if (fileAccess == FileAccess.Read && fileMode != FileMode.OpenOrCreate && fileMode != FileMode.Open)
        {
            throw new ArgumentOutOfRangeException(nameof(fileMode));
        }
        return Task.FromResult((Stream)File.Open(LocalPath, fileMode, fileAccess, FileShare.None));
    }
}
