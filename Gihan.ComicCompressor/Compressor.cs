using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gihan.ComicCompressor
{
    public abstract class Compressor
    {
        public uint Compressed { get; protected set; }
        public string Compressing { get; protected set; }

        public IEnumerable<DirectoryInfo> ComicChapterDirectoryInfos { get; }

        public Compressor(IEnumerable<DirectoryInfo> comicChapterDirectoryInfos)
        {
            ComicChapterDirectoryInfos = comicChapterDirectoryInfos;
            Compressed = 0;
        }
        public Compressor(IEnumerable<string> comicDirectoryPathes) :
            this(comicDirectoryPathes.Select(p => new DirectoryInfo(p)))
        { }

        public Compressor(DirectoryInfo comicDirectoryInfo) : this(comicDirectoryInfo.EnumerateDirectories()) { }
        public Compressor(string comicDirectoryPath) : this(new DirectoryInfo(comicDirectoryPath)) { }

        public abstract void Compress();

        private static Compressor GetCompressor(CompressionFormat format, object ctorInput)
        {
            switch (ctorInput)
            {
                case DirectoryInfo _:
                case string _:
                case IEnumerable<DirectoryInfo> _:
                case IEnumerable<string> _:
                    break;
                default:
                    throw new ArgumentException("Invalid type for ctorInput", nameof(ctorInput));
            }
            Type compressorType;
            switch (format)
            {
                case CompressionFormat.Zip:
                    compressorType = typeof(ZipCompressor);
                    break;
                case CompressionFormat.Rar:
                    compressorType = typeof(RarCompressor);
                    break;
                default:
                    throw new ArgumentException("Invalid format", nameof(format));
            }
            if (compressorType is null) throw new Exception();

            return (Compressor)compressorType.GetConstructor(new[] { ctorInput.GetType() })
                .Invoke(new[] { ctorInput });
        }

        public static Compressor GetCompressor(CompressionFormat format, DirectoryInfo comicDirectoryInfo)
        {
            return GetCompressor(format, comicDirectoryInfo as object);
        }
        public static Compressor GetCompressor(CompressionFormat format, string comicDirectoryPath)
        {
            return GetCompressor(format, comicDirectoryPath as object);
        }

        public static Compressor GetCompressor(CompressionFormat format, IEnumerable<DirectoryInfo> comicChapterDirectoryInfos)
        {
            return GetCompressor(format, comicChapterDirectoryInfos as object);
        }
        public static Compressor GetCompressor(CompressionFormat format, IEnumerable<string> comicDirectoryPathes)
        {
            return GetCompressor(format, comicDirectoryPathes as object);
        }
    }
}
