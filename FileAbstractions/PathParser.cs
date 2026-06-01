using System;

namespace FileAbstractions;

/// <summary>
/// Handles parsing of pure paths.
/// </summary>
/// <remarks>Use this instead of <see cref="System.IO.Path"/>, which is platform-dependant and intended for real (local) paths.</remarks>
internal static class PathParser
{
    public const char DIRECTORY_SEPARATOR_CHAR = '/';

    /// <summary>
    /// Makes absolute paths relative, and relative paths relative to the next subdirectory.
    /// For example: "/a/b/c" -> "a/b/c"; "a/b/c" -> "b/c"
    /// </summary>
    public static ReadOnlySpan<char> StripFirstComponent(ReadOnlySpan<char> path, out ReadOnlySpan<char> firstComponent)
    {
        int nextSeparator = path.IndexOf(DIRECTORY_SEPARATOR_CHAR);
        if (nextSeparator == -1)
        {
            firstComponent = path;
            return ReadOnlySpan<char>.Empty;
        }
        firstComponent = path[..nextSeparator];
        return path[(nextSeparator + 1)..];
    }
}
