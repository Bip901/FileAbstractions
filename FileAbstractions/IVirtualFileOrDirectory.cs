namespace FileAbstractions;

/// <summary>
/// The common ancestor of <see cref="IVirtualFile"/> and <see cref="IVirtualDirectory"/>.
/// </summary>
public interface IVirtualFileOrDirectory : IMovable, IHasFileAttributes { }
