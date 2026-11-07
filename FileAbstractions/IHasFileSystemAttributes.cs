using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions;

/// <summary>
/// An object which has a corresponding file system with <see cref="FileSystemAttributes"/>.
/// </summary>
public interface IHasFileSystemAttributes
{
    /// <summary>
    /// Gets the attributes of the file system this belongs to.
    /// </summary>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    Task<FileSystemAttributes> GetFileSystemAttributesAsync(CancellationToken cancellationToken);
}
