using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Gihan.ComicCompressor
{
    class ZipCompressor : Compressor
    {
        public ZipCompressor(DirectoryInfo ComicDirectoryInfo) : base(ComicDirectoryInfo)
        {
        }

        public ZipCompressor(string directoryPath) : base(directoryPath)
        {
        }

        public ZipCompressor(IEnumerable<DirectoryInfo> comicChapterDirectoryInfos) 
            : base(comicChapterDirectoryInfos)
        {
        }

        public ZipCompressor(IEnumerable<string> directoryPathes) : base(directoryPathes)
        {
        }

        public override void Compress()
        {
            foreach (var folder in ComicChapterDirectoryInfos)
            {
                try
                {
                    ZipFile.CreateFromDirectory(folder.FullName,
                        Path.Combine(folder.Parent.FullName, folder.Name + ".zip"));
                    Compressed++;
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
        }
    }
}
