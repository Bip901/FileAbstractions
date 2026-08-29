using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileAbstractions.Streams;

namespace FileAbstractions.Implementations.Streams;

/// <summary>
/// An extension of <see cref="FileStream"/> that adds <see cref="IConcurrentStream"/> APIs.
/// </summary>
public class ConcurrentFileStream : FileStream, IConcurrentStream
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentFileStream"/> class with the specified path, creation mode, read/write and sharing permission, the access other FileStreams can have to the same file, the buffer size, and additional file options.
    /// </summary>
    public ConcurrentFileStream(
        string path,
        FileMode mode,
        FileAccess access,
        FileShare share = FileShare.Read,
        int bufferSize = 4096,
        FileOptions options = FileOptions.None
    )
        : base(path, mode, access, share, bufferSize, options) { }

    /// <inheritdoc/>
    public ValueTask<int> ReadAtAsync(long offset, Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return RandomAccess.ReadAsync(SafeFileHandle, buffer, offset, cancellationToken);
    }

    /// <inheritdoc/>
    public ValueTask WriteAtAsync(
        long offset,
        ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = default
    )
    {
        return RandomAccess.WriteAsync(SafeFileHandle, buffer, offset, cancellationToken);
    }
}
