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

This library contains only abstractions, no concrete implementations.
