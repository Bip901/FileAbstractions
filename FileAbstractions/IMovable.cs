using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileAbstractions;

public interface IMovable
{
    /// <summary>
    /// Renames this file.
    /// </summary>
    /// <param name="newName">The new name and extension of this file.</param>
    /// <param name="allowOverwrite">If true, allows overwriting an existing file with the same name. Otherwise, throws an IOException if a file with the same name already exists.</param>
    /// <exception cref="FileNotFoundException">This file does not exist.</exception>
    /// <exception cref="IOException">There's already a file of the same name, or an IO exception occured.</exception>
    /// <exception cref="OperationCanceledException"/>
    Task RenameAsync(string newName, bool allowOverwrite, CancellationToken cancellationToken);

    /// <summary>
    /// Moves (renames) this file to a new location.
    /// </summary>>
    /// <param name="allowOverwrite">If true, allows overwriting an existing file with the same name. Otherwise, throws an IOException if a file with the same name already exists.</param>
    /// <exception cref="FileNotFoundException">This file does not exist.</exception>
    /// <exception cref="InvalidOperationException">When attempting to move under an incompatible parent.</exception>
    /// <exception cref="IOException">There's already a file of the same name, or an IO exception occured.</exception>
    /// <exception cref="OperationCanceledException"/>
    Task MoveToAsync(
        IVirtualDirectory newParent,
        string newName,
        bool allowOverwrite,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Deletes this file or directory. If this is a directory, it must be empty.
    /// </summary>
    /// <exception cref="FileNotFoundException">This file does not exist.</exception>
    /// <exception cref="IOException"/>
    /// <exception cref="OperationCanceledException"/>
    Task DeleteAsync(CancellationToken cancellationToken);
}
