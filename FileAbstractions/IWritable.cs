using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions;

/// <summary>
/// A file that can be written to.
/// </summary>
public interface IWritable
{
    /// <summary>
    /// Opens this file for writing.
    /// </summary>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The given file mode is not supported.</exception>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    /// <exception cref="IOException"/>
    Task<Stream> OpenWriteAsync(FileMode fileMode, CancellationToken cancellationToken);
}
