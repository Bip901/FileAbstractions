using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions.Implementations;

/// <summary>
/// A file that does not exist and never will.
/// </summary>
public class NonExistentFile : IVirtualFile
{
    /// <inheritdoc/>
    public Task DeleteAsync(CancellationToken cancellationToken)
    {
        throw new FileNotFoundException();
    }

    /// <inheritdoc/>
    public Task<FileAttributes> GetAttributesAsync(CancellationToken cancellationToken)
    {
        throw new FileNotFoundException();
    }

    /// <inheritdoc/>
    public Task MoveToAsync(IVirtualDirectory newParent, string newName, bool allowOverwrite, CancellationToken cancellationToken)
    {
        throw new FileNotFoundException();
    }

    /// <inheritdoc/>
    public Task RenameAsync(string newName, bool allowOverwrite, CancellationToken cancellationToken)
    {
        throw new FileNotFoundException();
    }

    /// <inheritdoc/>
    public Task SetAttributesAsync(FileAttributes attributes, CancellationToken cancellationToken)
    {
        throw new FileNotFoundException();
    }
}
