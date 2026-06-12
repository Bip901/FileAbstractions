using System;
using System.Collections.Generic;

namespace FileAbstractions;

/// <summary>
/// Handles parsing of pure paths.
/// </summary>
/// <remarks>Use this instead of <see cref="System.IO.Path"/>, which is platform-dependant and intended for real (local) paths.</remarks>
public static class PathParser
{
    /// <summary>
    /// The character that separates directories ('/'). This character must not appear in file names.
    /// </summary>
    public const char DIRECTORY_SEPARATOR_CHAR = '/';

    /// <summary>
    /// Makes absolute paths relative, and relative paths relative to the next subdirectory.
    /// For example: "/a/b/c" -> "a/b/c"; "a/b/c" -> "b/c"; "b/c" -> "c"; "c" -> ""
    /// </summary>
    public static ReadOnlySpan<char> StripFirstComponent(ReadOnlySpan<char> path, out ReadOnlySpan<char> firstComponent)
    {
        int nextSeparator = path.IndexOf(DIRECTORY_SEPARATOR_CHAR);
        if (nextSeparator == -1)
        {
            firstComponent = path;
            return [];
        }
        firstComponent = path[..nextSeparator];
        return path[(nextSeparator + 1)..];
    }

    /// <summary>
    /// Makes absolute paths relative, and relative paths relative to the next subdirectory.
    /// For example: "/a/b/c" -> "a/b/c"; "a/b/c" -> "b/c"; "b/c" -> "c"; "c" -> ""
    /// </summary>
    private static ReadOnlyMemory<char> StripFirstComponent(ReadOnlyMemory<char> path, out ReadOnlyMemory<char> firstComponent)
    {
        int nextSeparator = path.Span.IndexOf(DIRECTORY_SEPARATOR_CHAR);
        if (nextSeparator == -1)
        {
            firstComponent = path;
            return ReadOnlyMemory<char>.Empty;
        }
        firstComponent = path[..nextSeparator];
        return path[(nextSeparator + 1)..];
    }

    /// <summary>
    /// Yields, from beginning to end, the file and directory names in <paramref name="path"/>. For example "a/b/c" yields "a", "b", "c".
    /// Empty components raise <see cref="ArgumentException"/>, e.g. //a.txt, except for the empty component in the beginning of absolute paths (e.g. /a.txt), which is yielded.
    /// </summary>
    /// <param name="path">The path to parse.</param>
    /// <param name="allowTrailingSeparator">If true, allows a trailing directory separator, which will be ignored. Otherwise, raises <see cref="ArgumentException"/> if <paramref name="path"/> ends with a directory separator.
    /// Note that this includes the absolute root directory path, '/'.</param>
    /// <exception cref="ArgumentException"/>
    public static IEnumerable<ReadOnlyMemory<char>> IterateOverParts(string path, bool allowTrailingSeparator)
    {
        ReadOnlyMemory<char> pathMemory = path.AsMemory();
        if (path.EndsWith(DIRECTORY_SEPARATOR_CHAR))
        {
            if (!allowTrailingSeparator)
            {
                throw new ArgumentException(
                    $"Unexpected trailing '{DIRECTORY_SEPARATOR_CHAR}' in file path '{path}'",
                    nameof(path)
                );
            }
            pathMemory = pathMemory[..^1];
            if (pathMemory.IsEmpty)
            {
                // Path was '/', make sure to yield the empty component in the beginning
                yield return ReadOnlyMemory<char>.Empty;
                yield break;
            }
        }

        bool yieldedFirstComponent = false;
        while (!pathMemory.IsEmpty)
        {
            pathMemory = StripFirstComponent(pathMemory, out ReadOnlyMemory<char> firstComponent);
            if (firstComponent.IsEmpty && yieldedFirstComponent)
            {
                throw new ArgumentException($"Empty components are not allowed. Path: '{path}'");
            }
            yield return firstComponent;
            yieldedFirstComponent = true;
        }
    }

    /// <summary>
    /// Returns the last path component, for example "/a/b/c/" would return "c".
    /// </summary>
    /// <param name="path">The path to parse.</param>
    public static ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path)
    {
        _ = GetParentDirectory(path, out ReadOnlySpan<char> fileName);
        return fileName;
    }

    /// <summary>
    /// Returns the directory name of the given file, for example "/a/b/c" would return "/a/b".
    /// </summary>
    /// <remarks>A path with only one component would return an empty string.</remarks>
    /// <param name="path">The path to parse.</param>
    /// <param name="filename">The file name part of the path, for example "c".</param>
    /// <param name="allowTrailingSeparator">If true, allows a trailing directory separator, which will be ignored. Otherwise, raises <see cref="ArgumentException"/> if <paramref name="path"/> ends with a directory separator.</param>
    public static ReadOnlySpan<char> GetParentDirectory(
        ReadOnlySpan<char> path,
        out ReadOnlySpan<char> filename,
        bool allowTrailingSeparator = true
    )
    {
        if (path.EndsWith(DIRECTORY_SEPARATOR_CHAR))
        {
            if (!allowTrailingSeparator)
            {
                throw new ArgumentException(
                    $"Unexpected trailing '{DIRECTORY_SEPARATOR_CHAR}' in file path '{path}'",
                    nameof(path)
                );
            }
            path = path[..^1];
        }
        int lastSeparator = path.LastIndexOf(DIRECTORY_SEPARATOR_CHAR);
        filename = path[(lastSeparator + 1)..];
        if (lastSeparator == 0)
        {
            return path[..1];
        }
        else if (lastSeparator == -1)
        {
            return [];
        }
        else
        {
            return path[..lastSeparator];
        }
    }

    /// <summary>
    /// Returns whether the given path is within <paramref name="potentialParent"/>.
    /// For example, "/a/b" is relative to "/a".
    /// </summary>
    /// <remarks>Identical paths are considered relative to each other.</remarks>
    public static bool IsRelativeTo(ReadOnlySpan<char> path, ReadOnlySpan<char> potentialParent)
    {
        return TryGetRelativePath(path, potentialParent, out _);
    }

    /// <summary>
    /// If the given path is within <paramref name="potentialParent"/>, returns the relative path from <paramref name="potentialParent"/>.
    /// For example, "/a/b" relative to "/a" is "b".
    /// </summary>
    /// <remarks>If the two paths are identical, an empty string is returned.
    /// A trailing '/' on <paramref name="potentialParent"/> is allowed and ignored. A trailing '/' on <paramref name="path"/> determines whether a trailing '/' will appear in the return value of this method.</remarks>
    /// <exception cref="InvalidOperationException">The given path is not relative to <paramref name="potentialParent"/></exception>
    public static ReadOnlySpan<char> GetRelativePath(ReadOnlySpan<char> path, ReadOnlySpan<char> potentialParent)
    {
        if (!TryGetRelativePath(path, potentialParent, out ReadOnlySpan<char> relativePath))
        {
            throw new InvalidOperationException(
                $"The given path '{path}' is not relative to the potential parent '{potentialParent}'."
            );
        }
        return relativePath;
    }

    /// <summary>
    /// If the given path is within <paramref name="potentialParent"/>, sets <paramref name="relativePath"/> to the relative path from <paramref name="potentialParent"/>.
    /// For example, "/a/b" relative to "/a" is "b".
    /// </summary>
    /// <returns>True if succeeded, false if the given path is not relative to <paramref name="potentialParent"/>.</returns>
    /// <remarks>If the two paths are identical, an empty string is returned.
    /// A trailing '/' on <paramref name="potentialParent"/> is allowed and ignored. A trailing '/' on <paramref name="path"/> determines whether a trailing '/' will appear in <paramref name="relativePath"/>.</remarks>
    public static bool TryGetRelativePath(
        ReadOnlySpan<char> path,
        ReadOnlySpan<char> potentialParent,
        out ReadOnlySpan<char> relativePath
    )
    {
        if (potentialParent.IsEmpty)
        {
            if (IsAbsolute(path))
            {
                relativePath = [];
                return false;
            }
            relativePath = path;
            return true;
        }
        potentialParent = TrimTrailingSlash(potentialParent);
        if (!path.StartsWith(potentialParent))
        {
            relativePath = [];
            return false;
        }
        if (potentialParent.Length == path.Length)
        {
            relativePath = [];
            return true;
        }
        if (potentialParent.Length == 1 && potentialParent[0] == DIRECTORY_SEPARATOR_CHAR)
        {
            relativePath = path[1..];
            return true;
        }
        if (path[potentialParent.Length] != DIRECTORY_SEPARATOR_CHAR)
        {
            relativePath = [];
            return false;
        }
        relativePath = path[(potentialParent.Length + 1)..];
        return true;
    }

    /// <summary>
    /// Returns whether the given path is absolute (e.g. "/a/b") or relative (e.g. "a/b").
    /// </summary>
    public static bool IsAbsolute(ReadOnlySpan<char> path)
    {
        return path.Length > 0 && path[0] == DIRECTORY_SEPARATOR_CHAR;
    }

    private static ReadOnlySpan<char> TrimTrailingSlash(ReadOnlySpan<char> path)
    {
        if (path.Length > 1 && path[^1] == DIRECTORY_SEPARATOR_CHAR)
        {
            return path[..^1];
        }
        return path;
    }
}
