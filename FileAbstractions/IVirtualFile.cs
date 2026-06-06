namespace FileAbstractions;

/// <summary>
/// An abstract file. This interface is the most generic one, assuming only that the file is a tree item with attributes.
/// Files usually implement <see cref="IReadable"/>, <see cref="IWritable"/>, <see cref="IReadableWriteable"/>, etc.
/// Callers then cast objects implementing <see cref="IVirtualFile"/> to these other interfaces, for example:
/// <code>if (file is IReadable readable) { readable.OpenReadAsync(...); ... }</code>
/// </summary>
public interface IVirtualFile : IVirtualFileOrDirectory { }
