namespace FileAbstractions.Streams;

/// <summary>
/// A thread-safe, random-access open file which supports both reading and writing.
/// </summary>
public interface IConcurrentStream : IConcurrentReadableStream, IConcurrentWritableStream { }
