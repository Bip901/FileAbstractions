namespace FileAbstractions;

/// <summary>
/// A <see cref="IReadable"/> which also exposes a name and size attributes.
/// </summary>
public interface INamedSizedReadable : IReadable
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
