using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions;

public interface IHasFileAttributes
{
    /// <summary>
    /// Gets the filesystem attributes of this file.
    /// </summary>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="FileNotFoundException">If this file does not exist.</exception>
    /// <exception cref="DirectoryNotFoundException"/>
    Task<FileAttributes> GetAttributesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Sets the filesystem attributes of this file.
    /// Currently only the last modified time should be implemented.
    /// </summary>
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="FileNotFoundException">If this file does not exist.</exception>
    /// <exception cref="DirectoryNotFoundException"/>
    Task SetAttributesAsync(FileAttributes attributes, CancellationToken cancellationToken);
}
