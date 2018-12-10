using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Gihan.ComicCompressor
{
    class RarCompressor : Compressor
    {
        public RarCompressor(DirectoryInfo ComicDirectoryInfo) : base(ComicDirectoryInfo)
        {
        }

        public RarCompressor(string directoryPath) : base(directoryPath)
        {
        }

        public RarCompressor(IEnumerable<DirectoryInfo> comicChapterDirectoryInfos) 
            : base(comicChapterDirectoryInfos)
        {
        }

        public RarCompressor(IEnumerable<string> directoryPathes) : base(directoryPathes)
        {
        }

        public override void Compress()
        {
            foreach (var folder in ComicChapterDirectoryInfos)
            {
                try
                {
                    var process = new Process();
                    process.StartInfo.FileName = Path.Combine(Environment.CurrentDirectory, "Assets", "rar.exe");
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
    }
}
