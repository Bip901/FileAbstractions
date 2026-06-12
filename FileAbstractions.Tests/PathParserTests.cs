using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FileAbstractions.Tests;

public class PathParserTests
{
    [Theory]
    [InlineData("/a/b", true)]
    [InlineData("/", true)]
    [InlineData("a/b", false)]
    [InlineData("", false)]
    public void IsAbsolute_IdentifiesCorrectly(string path, bool expected)
    {
        bool result = PathParser.IsAbsolute(path.AsSpan());

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("/a/b/c", "a/b/c", "")]
    [InlineData("a/b/c", "b/c", "a")]
    [InlineData("b/c", "c", "b")]
    [InlineData("c", "", "c")]
    [InlineData("", "", "")]
    public void StripFirstComponent_SlicesCorrectly(string input, string expectedRemainder, string expectedFirst)
    {
        ReadOnlySpan<char> remainder = PathParser.StripFirstComponent(
            input.AsSpan(),
            out ReadOnlySpan<char> firstComponent
        );

        Assert.Equal(expectedRemainder, remainder.ToString());
        Assert.Equal(expectedFirst, firstComponent.ToString());
    }

    [Theory]
    [InlineData("/a/b/c/", "c")]
    [InlineData("/a/b/c", "c")]
    [InlineData("c", "c")]
    [InlineData("/", "")]
    [InlineData("", "")]
    public void GetFileName_ReturnsLastComponent(string input, string expected)
    {
        ReadOnlySpan<char> filename = PathParser.GetFileName(input.AsSpan());

        Assert.Equal(expected, filename.ToString());
    }

    [Theory]
    [InlineData("/a/b/c", "/a/b", "c")]
    [InlineData("a/b/c", "a/b", "c")]
    [InlineData("/a", "/", "a")]
    [InlineData("a", "", "a")]
    [InlineData("/", "", "")]
    public void GetParentDirectory_ParsesValidPaths(string input, string expectedDir, string expectedFile)
    {
        ReadOnlySpan<char> directory = PathParser.GetParentDirectory(
            input.AsSpan(),
            out ReadOnlySpan<char> filename,
            true
        );

        Assert.Equal(expectedDir, directory.ToString());
        Assert.Equal(expectedFile, filename.ToString());
    }

    [Fact]
    public void GetParentDirectory_ThrowsException_WhenTrailingSeparatorNotAllowed()
    {
        Assert.Throws<ArgumentException>(() => PathParser.GetParentDirectory("/a/b/".AsSpan(), out _, false));
    }

    [Theory]
    [InlineData("/a/b", "/a", true)]
    [InlineData("/a/b/", "/a", true)]
    [InlineData("/a", "/a", true)]
    [InlineData("/ab", "/a", false)]
    [InlineData("a/b", "a", true)]
    [InlineData("/a/b", "a", false)]
    [InlineData("a/b", "", true)]
    [InlineData("/a/b", "", false)]
    public void IsRelativeTo_ValidatesHierarchies(string path, string potentialParent, bool expected)
    {
        bool result = PathParser.IsRelativeTo(path.AsSpan(), potentialParent.AsSpan());

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("/a/b", "/a", "b")]
    [InlineData("/a/b/", "/a", "b/")]
    [InlineData("/a", "/a", "")]
    [InlineData("/", "/", "")]
    [InlineData("a/b", "a", "b")]
    [InlineData("a/b", "", "a/b")]
    public void GetRelativePath_ReturnsCorrectPath(string path, string potentialParent, string expected)
    {
        ReadOnlySpan<char> result = PathParser.GetRelativePath(path.AsSpan(), potentialParent.AsSpan());

        Assert.Equal(expected, result.ToString());
    }

    [Theory]
    [InlineData("/ab", "/a")]
    [InlineData("/a/b", "/b")]
    [InlineData("/a/b", "")]
    public void GetRelativePath_ThrowsException_WhenNotRelative(string path, string potentialParent)
    {
        Assert.Throws<InvalidOperationException>(() =>
            PathParser.GetRelativePath(path.AsSpan(), potentialParent.AsSpan())
        );
    }

    [Theory]
    [InlineData("a/b/c", true, new[] { "a", "b", "c" })]
    [InlineData("/a/b/c", true, new[] { "", "a", "b", "c" })]
    [InlineData("/", true, new[] { "" })]
    [InlineData("a/b/c/", true, new[] { "a", "b", "c" })]
    [InlineData("", true, new string[0])]
    public void IterateOverParts_YieldsExpectedComponents(string path, bool allowTrailing, string[] expected)
    {
        IEnumerable<ReadOnlyMemory<char>> components = PathParser.IterateOverParts(path, allowTrailing);
        string[] actual = components.Select(string (ReadOnlyMemory<char> m) => m.ToString()).ToArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IterateOverParts_ThrowsException_WhenTrailingSeparatorForbidden()
    {
        Assert.Throws<ArgumentException>(() => PathParser.IterateOverParts("/a/b/", false).ToList());
    }

    [Theory]
    [InlineData("a//b")]
    [InlineData("/a//b")]
    public void IterateOverParts_ThrowsException_OnInternalEmptyComponents(string path)
    {
        Assert.Throws<ArgumentException>(() => PathParser.IterateOverParts(path, true).ToList());
    }
}
