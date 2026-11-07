# File Abstractions

A C# .NET library for async filesystem abstractions.

## Purpose

Filesystems have become practically the only way modern software interacts with storage media.
While the operating system provides an API to list, create and work with files, this API is mostly made for local disks, or rarely network mounts of specific protocols - but mounting these requires special privleges which are sometimes not available at all (for example, in mobile devices).

Moreover, Android has [blocked access](https://source.android.com/docs/core/storage/scoped) to the OS filesystem API in favor of "Scoped Storage" (which **lacks** features such as setting a file's last modified date, but that's beside the point).

There's a huge variety of different implementations of very similar concepts:
* SFTP
* FTP(S)
* WebDAV
* Proprietary Cloud Storage protocols (Google Drive, Microsoft OneDrive...)
* And many more.

Therefore, it's logical to do the filesystem abstraction at the application layer instead of relying on the operating system, and thus these interfaces were made.

## Implementation Notes

### Path Format

* The contract of all interfaces is to use [pure virtual paths](FileAbstractions/PathParser.cs), where directories are separated with a forward slash ('/'), regardless of the operating system.
    * Despite this, [LocalDirectory](./FileAbstractions/Implementations/LocalDirectory.cs) sanitizes '\\' on Windows to prevent unexpected behavior.
    
    * A trailing '/' in directory paths is allowed in some contexts.

* Special directories "." and ".." are deliberately **not** supported.

### Files and Directories API

* Users of this library hold [IVirtualFile](./FileAbstractions/IVirtualFile.cs) or [IVirtualDirectory](./FileAbstractions/IVirtualDirectory.cs) instances, matching the file type they expect to be found. These are roughly equivalent to paths: the file or directory being pointed to doesn't necessarily exist, nor is it even of the expected type, since the filesystem could have been modified since the last time we checked it. Therefore, callers should expect `FileNotFoundException` and `DirectoryNotFoundException`.

* Despite this, when opening a file, the returned `Stream` instance is equivalent to an open file descriptor, and will always refer to the same file it referred to at the moment of opening.

* Returned streams may implement [IHasFileAttributes](./FileAbstractions/IHasFileAttributes.cs) to allow callers to inspect the attributes of the original file, similar to `fstat`.

* Returned streams may implement [IConcurrentStream](./FileAbstractions/IConcurrentStream.cs) to allow callers to invoke multiple read/write calls concurrently and statelessly, as opposed to the usual, sequential `Stream` interface.

* Returned streams may implement [IAsyncDisposableCancelable](./FileAbstractions/IAsyncDisposableCancelable.cs), where the caller can supply a `CancellationToken` to `DisposeAsync` to cancel the disposal. This is useful for example if the disposal itself requires network IO.
