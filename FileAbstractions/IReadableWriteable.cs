using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions;

/// <summary>
/// A file that can be both read from and written to.
/// </summary>
public interface IReadableWriteable : IReadable, IWritable
{
    /// <summary>
    /// Opens this file for reading or writing.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The given file mode is not supported.</exception>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    /// <exception cref="IOException"/>
    Task<Stream> OpenAsync(
        FileMode fileMode,
        FileAccess fileAccess,
        StreamOptions options,
        CancellationToken cancellationToken
    )
    {
        if (fileAccess == FileAccess.ReadWrite)
        {
            return OpenReadWriteAsync(fileMode, options, cancellationToken);
        }
        else if (fileAccess == FileAccess.Write)
        {
            return OpenWriteAsync(fileMode, options, cancellationToken);
        }
        else
        {
            return OpenReadAsync(fileMode, options, cancellationToken);
        }
    }

    /// <summary>
    /// Opens this file for reading and writing.
    /// </summary>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The given file mode is not supported.</exception>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    /// <exception cref="IOException"/>
    Task<Stream> OpenReadWriteAsync(FileMode fileMode, StreamOptions options, CancellationToken cancellationToken);
}
