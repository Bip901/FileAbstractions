using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions.Streams;

/// <summary>
/// A thread-safe, random-access open file which supports reading.
/// </summary>
public interface IConcurrentReadableStream : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// The size, in bytes, of this stream.
    /// </summary>
    long Length { get; }

    /// <summary>
    /// Asynchronously reads a sequence of bytes from this stream.
    /// </summary>
    /// <param name="offset">The offset to start reading from.</param>
    /// <param name="buffer">The region of memory to write the data into.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous read operation. The value of its Result property contains the total number of bytes read into the buffer. The result value can be less than the length of the buffer if that many bytes are not currently available, or it can be 0 (zero) if the length of the buffer is 0 or if the end of the stream has been reached.</returns>
    /// <exception cref="OperationCanceledException"/>
    ValueTask<int> ReadAtAsync(long offset, Memory<byte> buffer, CancellationToken cancellationToken = default);
}
