namespace FileAbstractions;

public interface INamedSizedReadableFile : IReadable
{
    /// <summary>
    /// The name of this file (including the extension).
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The size of this file in bytes.
    /// </summary>
    ulong Size { get; }
}
