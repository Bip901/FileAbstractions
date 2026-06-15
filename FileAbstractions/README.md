# File Abstractions

A C# .NET library for async filesystem abstractions.

## Purpose

Operating system filesystem APIs are primarily designed for local disks (network mounts, even when available, often require elevated privileges to mount).
Furthermore, operating systems like Android restrict traditional filesystem access through mechanisms like "Scoped Storage."

To overcome these limitations and unify the wide variety of storage protocols - e.g. SFTP, FTP(S), WebDAV, proprietary cloud storage (e.g., Google Drive, OneDrive), etc - this library abstracts filesystem operations at the application layer rather than relying on the underlying OS.

More information available in the [GitHub repository](https://github.com/Bip901/FileAbstractions).
