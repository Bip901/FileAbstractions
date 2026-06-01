using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions;

public interface IVirtualDirectory : IVirtualFileOrDirectory
{
    /// <summary>
    /// Returns the child file or directory with the given name.
    /// </summary>
    /// <exception cref="FileNotFoundException"/>
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
    IVirtualDirectory GetDescendantDirectory(ReadOnlySpan<char> relativePath)
    {
        if (relativePath.IsEmpty)
        {
            return this;
        }
        relativePath = PathParser.StripFirstComponent(relativePath, out ReadOnlySpan<char> firstComponent);
        return GetChildDir(firstComponent).GetDescendantDirectory(relativePath);
    }

    /// <summary>
    /// Returns the descendant file specified by the given relative path, for example a/b/c.
    /// </summary>
    /// <remarks>
    /// The default implementation is slow.
    /// Implementers of this interface should override the default implementation if they can short-circuit it
    /// and return a result directly, to avoid recursive virtual calls and allocating an object for each intermediate directory.
    /// </remarks>
    IVirtualFile GetDescendantFile(ReadOnlySpan<char> relativePath)
    {
        if (relativePath.IsEmpty)
        {
            throw new ArgumentException(null, nameof(relativePath));
        }
        if (!relativePath.Contains(PathParser.DIRECTORY_SEPARATOR_CHAR))
        {
            return GetChildFile(relativePath);
        }
        relativePath = PathParser.StripFirstComponent(relativePath, out ReadOnlySpan<char> firstComponent);
        return GetChildDir(firstComponent).GetDescendantFile(relativePath);
    }
}
