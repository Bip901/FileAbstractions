using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions.Streams;

/// <summary>
/// A thread-safe, random-access open file which supports writing.
/// </summary>
public interface IConcurrentWritableStream : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Asynchronously writes a sequence of bytes to this stream.
    /// </summary>
    /// <param name="offset">The offset to start writing to.</param>
    /// <param name="buffer">The region of memory to write data from.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="OperationCanceledException"/>
    ValueTask WriteAtAsync(long offset, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous flush operation.</returns>
    Task FlushAsync(CancellationToken cancellationToken = default);
}
