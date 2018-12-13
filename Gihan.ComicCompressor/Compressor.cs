using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Gihan.ComicCompressor
{
    public class Compressor
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

        public Compressor(DirectoryInfo comicDirectoryInfo) 
            : this(comicDirectoryInfo.EnumerateDirectories().
                  Where(d => (d.Attributes & FileAttributes.System) == 0)) { }
        public Compressor(string comicDirectoryPath) : this(new DirectoryInfo(comicDirectoryPath)) { }

        public void RarCompress()
        {
            foreach (var folder in ComicChapterDirectoryInfos)
            {
                try
                {
                    var process = new Process();
                    var rarFile = Directory.GetFiles(Environment.CurrentDirectory, 
                        "*rar.exe*", SearchOption.AllDirectories);
                    process.StartInfo.FileName = rarFile[0];
                    process.StartInfo.WorkingDirectory = folder.FullName;
                    process.StartInfo.Arguments = $"a -r \"..\\{folder.Name}.rar\" *";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    var output = process.StandardOutput.ReadToEnd();
                    Compressed++;
                }
                catch (Exception err)
                {

                    throw err;
                }
            }
        }

        public void ZipCompress()
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

        public void Compress(CompressionFormat format)
        {
            switch (format)
            {
                case CompressionFormat.Zip:
                    ZipCompress();
                    break;
                case CompressionFormat.Rar:
                    RarCompress();
                    break;
                default:
                    break;
            }
        }

        //private static Compressor GetCompressor(CompressionFormat format, object ctorInput)
        //{
        //    switch (ctorInput)
        //    {
        //        case DirectoryInfo _:
        //        case string _:
        //        case IEnumerable<DirectoryInfo> _:
        //        case IEnumerable<string> _:
        //            break;
        //        default:
        //            throw new ArgumentException("Invalid type for ctorInput", nameof(ctorInput));
        //    }
        //    Type compressorType;
        //    switch (format)
        //    {
        //        case CompressionFormat.Zip:
        //            compressorType = typeof(ZipCompressor);
        //            break;
        //        case CompressionFormat.Rar:
        //            compressorType = typeof(RarCompressor);
        //            break;
        //        default:
        //            throw new ArgumentException("Invalid format", nameof(format));
        //    }
        //    if (compressorType is null) throw new Exception();

        //    return (Compressor)compressorType.GetConstructor(new[] { ctorInput.GetType() })
        //        .Invoke(new[] { ctorInput });
        //}

        //public static Compressor GetCompressor(CompressionFormat format, DirectoryInfo comicDirectoryInfo)
        //{
        //    return GetCompressor(format, comicDirectoryInfo as object);
        //}
        //public static Compressor GetCompressor(CompressionFormat format, string comicDirectoryPath)
        //{
        //    return GetCompressor(format, comicDirectoryPath as object);
        //}

        //public static Compressor GetCompressor(CompressionFormat format, IEnumerable<DirectoryInfo> comicChapterDirectoryInfos)
        //{
        //    return GetCompressor(format, comicChapterDirectoryInfos as object);
        //}
        //public static Compressor GetCompressor(CompressionFormat format, IEnumerable<string> comicDirectoryPathes)
        //{
        //    return GetCompressor(format, comicDirectoryPathes as object);
        //}
    }
}
