using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions;

public interface IReadable
{
    /// <summary>
    /// Opens this file for reading.
    /// </summary>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The given file mode is not supported.</exception>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    /// <exception cref="IOException"/>
    Task<Stream> OpenReadAsync(FileMode fileMode, CancellationToken cancellationToken);
}
