using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions.Implementations;

/// <summary>
/// A directory that does not exist and never will.
/// </summary>
public class NonExistentDirectory : IVirtualDirectory
{
    /// <inheritdoc/>
    public Task DeleteAsync(CancellationToken cancellationToken)
    {
        throw new DirectoryNotFoundException();
    }

    /// <inheritdoc/>
    public Task<FileAttributes> GetAttributesAsync(CancellationToken cancellationToken)
    {
        throw new DirectoryNotFoundException();
    }

    /// <inheritdoc/>
    public IVirtualDirectory GetChildDir(ReadOnlySpan<char> name)
    {
        return this;
    }

    /// <inheritdoc/>
    public IVirtualFile GetChildFile(ReadOnlySpan<char> name)
    {
        return new NonExistentFile();
    }

    /// <inheritdoc/>
    public IVirtualFileOrDirectory GetExistingChild(ReadOnlySpan<char> name)
    {
        throw new DirectoryNotFoundException();
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<FileEntry> ListChildren(CancellationToken cancellationToken)
    {
        throw new DirectoryNotFoundException();
    }

    /// <inheritdoc/>
    public Task<IVirtualDirectory> MakeDirAsync(
        ReadOnlySpan<char> name,
        FileAttributes attributes,
        CancellationToken cancellationToken
    )
    {
        throw new DirectoryNotFoundException();
    }

    /// <inheritdoc/>
    public Task MoveToAsync(
        IVirtualDirectory newParent,
        string newName,
        bool allowOverwrite,
        CancellationToken cancellationToken
    )
    {
        throw new DirectoryNotFoundException();
    }

    /// <inheritdoc/>
    public Task RenameAsync(string newName, bool allowOverwrite, CancellationToken cancellationToken)
    {
        throw new DirectoryNotFoundException();
    }

    /// <inheritdoc/>
    public Task SetAttributesAsync(FileAttributes attributes, CancellationToken cancellationToken)
    {
        throw new DirectoryNotFoundException();
    }
}
