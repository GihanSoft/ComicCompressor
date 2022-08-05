using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicC.Logics;

public static class ComicCompressorEngine
{
    public static async Task CompressComicsAsync(IEnumerable<ChapterVM> pathes, bool removeSource, IProgress<ChapterVM> progress)
    {
        var tasks = pathes.Select(async item =>
        {
            item.Status = ChapterVM.ChapterStatus.Started;
            try
            {
                await CreateZipFileFromDirectoryAsync(
                    item.Path,
                    item.Path + ".cbz",
                    CompressionLevel.NoCompression,
                    false)
                    .ConfigureAwait(false);

                if (removeSource)
                {
                    Directory.Delete(item.Path, true);
                }
            }
            catch (Exception ex)
            {
                item.Status = ChapterVM.ChapterStatus.Failed;
                item.Exception = ex;
                throw;
            }

            item.Status = ChapterVM.ChapterStatus.Succeed;
            progress?.Report(item);
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private static async Task CreateZipFileFromDirectoryAsync(
        string sourceDirectoryName,
        string destinationArchiveFileName,
        CompressionLevel compressionLevel,
        bool includeBaseDirectory)
    {
        using var archive = ZipFile.Open(destinationArchiveFileName, ZipArchiveMode.Create);

        DirectoryInfo dirInfo = new(sourceDirectoryName);
        var storageItems = dirInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories);

        var directoryIsEmpty = false;
        foreach (var item in storageItems)
        {
            directoryIsEmpty = false;
            var baseLength = includeBaseDirectory ? dirInfo.Parent.FullName.Length : dirInfo.FullName.Length;

            var path = item.FullName
                .Substring(
                    baseLength,
                    item.FullName.Length - baseLength)
                .TrimStart('/', '\\');

            switch (item)
            {
                case FileInfo fileInfo:
                    var entry = archive.CreateEntry(path, compressionLevel);
                    using (var entryStream = entry.Open())
                    using (var fileStream = fileInfo.OpenRead())
                    {
                        await fileStream.CopyToAsync(entryStream).ConfigureAwait(false);
                    }

                    break;
                case DirectoryInfo directoryInfo when !directoryInfo.EnumerateFileSystemInfos().Any():
                    _ = archive.CreateEntry(path + "/", compressionLevel);
                    break;
                default:
                    break;
            }
        }

        if (includeBaseDirectory && directoryIsEmpty)
        {
            _ = archive.CreateEntry(dirInfo.Name + "/", compressionLevel);
        }
    }
}
