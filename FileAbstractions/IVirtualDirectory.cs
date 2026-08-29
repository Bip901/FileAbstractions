using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions;

/// <summary>
/// A directory, which is a special file that aggregates other files within it.
/// </summary>
public interface IVirtualDirectory : IVirtualFileOrDirectory
{
    // TODO: Replace with an async API or find another solution
    /// <summary>
    /// Returns the child file or directory with the given name.
    /// </summary>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="NotSupportedException"/>
    /// <remarks>
    /// This method will be deprecated in a future release and should not be implemented.
    /// The point of this API is to get an instance in a non-async way, where the caller only
    /// needs to use methods of <see cref="IVirtualFileOrDirectory"/> and doesn't know (or care) whether the item is a file
    /// or a directory. Since I don't want to return a concrete instance that implements <see cref="IVirtualFileOrDirectory"/>
    /// but is neither a <see cref="IVirtualFile"/> nor a <see cref="IVirtualDirectory"/>, this requires querying the filesystem
    /// to decide which concrete instance to return. In non-local filesystems, this cannot be achieved in a blocking (not async) way.
    /// </remarks>
    IVirtualFileOrDirectory GetExistingChild(ReadOnlySpan<char> name);

    /// <summary>
    /// Returns an <see cref="IVirtualFile"/> that represents this directory's child named <paramref name="name"/>.
    /// The file may or may not exist at the time this method is called.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="name"/> is not a valid file name, e.g. empty.</exception>
    IVirtualFile GetChildFile(ReadOnlySpan<char> name);

    /// <summary>
    /// Returns an <see cref="IVirtualDirectory"/> that represents this directory's child named <paramref name="name"/>.
    /// The directory may or may not exist at the time this method is called.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="name"/> is not a valid file name, e.g. empty.</exception>
    IVirtualDirectory GetChildDir(ReadOnlySpan<char> name);

    /// <summary>
    /// Creates an empty directory.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException"/>
    /// <exception cref="IOException"/>
    /// <exception cref="OperationCanceledException"/>
    /// <returns>An object representing the created directory.</returns>
    Task<IVirtualDirectory> MakeDirAsync(
        ReadOnlySpan<char> name,
        FileAttributes attributes,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Enumerates the direct children of this directory.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException"/>
    IAsyncEnumerable<FileEntry> ListChildren(CancellationToken cancellationToken);

    /// <summary>
    /// Returns the descendant directory specified by the given relative path, for example a/b/c, where an empty string returns this.
    /// </summary>
    /// <remarks>
    /// The default implementation is slow.
    /// Implementers of this interface should override the default implementation if they can short-circuit it
    /// and return a result directly, to avoid recursive virtual calls and allocating an object for each intermediate directory.
    /// </remarks>
    /// <exception cref="ArgumentException"/>
    IVirtualDirectory GetDescendantDirectory(ReadOnlySpan<char> relativePath)
    {
        if (relativePath.IsEmpty)
        {
            return this;
        }
        relativePath = PathParser.StripFirstComponent(relativePath, out ReadOnlySpan<char> firstComponent);
        if (firstComponent.IsEmpty)
        {
            throw new ArgumentException("Empty subdirectory names are not allowed.", nameof(relativePath));
        }
        return GetChildDir(firstComponent).GetDescendantDirectory(relativePath);
    }

    /// <summary>
    /// Returns the descendant file specified by the given relative path, for example a/b/c.
    /// </summary>
    /// <remarks>
    /// The default implementation is slow (although not as slow if <see cref="GetDescendantDirectory"/> is overridden).
    /// Implementers of this interface should override the default implementation if they can short-circuit it
    /// and return a result directly, to avoid recursive virtual calls and allocating an object for each intermediate directory.
    /// </remarks>
    /// <exception cref="ArgumentException"/>
    IVirtualFile GetDescendantFile(ReadOnlySpan<char> relativePath)
    {
        if (relativePath.IsEmpty)
        {
            throw new ArgumentException("Path is empty", nameof(relativePath));
        }
        ReadOnlySpan<char> parentName = PathParser.GetParentDirectory(
            relativePath,
            out ReadOnlySpan<char> fileName,
            allowTrailingSeparator: false
        );
        return GetDescendantDirectory(parentName).GetChildFile(fileName);
    }
}
