using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions.Implementations;

/// <summary>
/// An implementation of <see cref="IVirtualDirectory"/> backed by the operating system's filesystem.
/// </summary>
public class LocalDirectory(string localPath) : LocalFileOrDirectory(localPath), IVirtualDirectory
{
    /// <inheritdoc/>
    public override Task DeleteAsync(CancellationToken cancellationToken)
    {
        Directory.Delete(LocalPath);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override void MoveTo(string newLocalPath, bool allowOverwrite)
    {
        if (allowOverwrite)
        {
            try
            {
                Directory.Delete(newLocalPath);
            }
            catch (DirectoryNotFoundException)
            {
                // Good, no need to delete
            }
        }
        Directory.Move(LocalPath, newLocalPath);
    }

    /// <inheritdoc/>
    protected override FileSystemInfo GetFileSystemInfo()
    {
        if (!Directory.Exists(LocalPath))
        {
            throw new DirectoryNotFoundException($"Directory {LocalPath} does not exist");
        }
        return new DirectoryInfo(LocalPath);
    }

    /// <inheritdoc/>
    public IVirtualFileOrDirectory GetExistingChild(ReadOnlySpan<char> name)
    {
        SanitizeName(name);
        string localPath = Path.Join(LocalPath, name);
        if (File.Exists(localPath))
        {
            return new LocalFile(localPath);
        }
        else if (Directory.Exists(localPath))
        {
            return new LocalDirectory(localPath);
        }
        throw new FileNotFoundException();
    }

    /// <inheritdoc/>
    public IVirtualFile GetChildFile(ReadOnlySpan<char> name)
    {
        SanitizeName(name);
        return new LocalFile(Path.Join(LocalPath, name));
    }

    /// <inheritdoc/>
    public IVirtualDirectory GetChildDir(ReadOnlySpan<char> name)
    {
        SanitizeName(name);
        return new LocalDirectory(Path.Join(LocalPath, name));
    }

    /// <inheritdoc/>
    public Task<IVirtualDirectory> MakeDirAsync(
        ReadOnlySpan<char> name,
        FileAttributes attributes,
        CancellationToken cancellationToken
    )
    {
        SanitizeName(name);
        string newDirPath = Path.Join(LocalPath, name);
        DirectoryInfo dirInfo = Directory.CreateDirectory(newDirPath);
        SetAttributes(dirInfo, attributes);
        return Task.FromResult((IVirtualDirectory)new LocalDirectory(newDirPath));
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<FileEntry> ListChildren([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var fileSystemInfo in new DirectoryInfo(LocalPath).GetFileSystemInfos())
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return FileEntry.FromFileSystemInfo(fileSystemInfo);
        }
    }

    /// <inheritdoc/>
    public IVirtualDirectory GetDescendantDirectory(ReadOnlySpan<char> relativePath)
    {
        SanitizeRelativePath(relativePath);
        return new LocalDirectory(Path.Join(LocalPath, relativePath));
    }

    /// <inheritdoc/>
    public IVirtualFile GetDescendantFile(ReadOnlySpan<char> relativePath)
    {
        SanitizeRelativePath(relativePath);
        return new LocalFile(Path.Join(LocalPath, relativePath));
    }

    private static void SanitizeRelativePath(ReadOnlySpan<char> relativePath)
    {
        if (Path.IsPathRooted(relativePath))
        {
            throw new ArgumentException($"Path must be relative: '{relativePath}'", nameof(relativePath));
        }
        if (
            Path.DirectorySeparatorChar != PathParser.DIRECTORY_SEPARATOR_CHAR
            && relativePath.Contains(Path.DirectorySeparatorChar)
        )
        {
            throw new ArgumentException(
                $"The current operating system does not allow '{Path.DirectorySeparatorChar}' in file names",
                nameof(relativePath)
            );
        }
        ReadOnlySpan<char> remaining = relativePath;
        while (!remaining.IsEmpty)
        {
            remaining = PathParser.StripFirstComponent(remaining, out ReadOnlySpan<char> component);
            if (component == ".")
            {
                throw new ArgumentException($"File name '.' is not supported: '{relativePath}'", nameof(relativePath));
            }
            if (component == "..")
            {
                throw new ArgumentException($"File name '..' is not supported: '{relativePath}'", nameof(relativePath));
            }
        }
    }
}
