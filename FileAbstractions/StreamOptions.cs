using System.IO;

namespace FileAbstractions;

/// <summary>
/// Options for opening a stream (e.g. <see cref="IReadable"/> or <see cref="IWritable"/>).
/// These are merely a hint; The stream opener may interpret them as desired.
/// </summary>
public sealed class StreamOptions
{
    /// <summary>
    /// Whether the caller plans to call <see cref="Stream.Seek"/> on the returned stream.
    /// </summary>
    public bool SeekingDesired { get; set; }
}
