using System;
using System.Runtime;
using System.Windows;

using static System.Environment;

namespace Gihan.ComicCompressor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                string path = null;
                CompressionFormat? format = null;

                for (int i = 0; i < e.Args.Length; i++)
                {
                    if (!e.Args[i].StartsWith("-"))
                    {
                        path = e.Args[i];
                        continue;
                    }
                    if (e.Args[i].Equals("-p", StringComparison.OrdinalIgnoreCase) ||
                       e.Args[i].Equals("--path", StringComparison.OrdinalIgnoreCase))
                    {
                        path = e.Args[i + 1];
                        i++; continue;
                    }
                    if (e.Args[i].Equals("-z", StringComparison.OrdinalIgnoreCase) ||
                        e.Args[i].Equals("--zip", StringComparison.OrdinalIgnoreCase))
                    {
                        format = CompressionFormat.Zip;
                        continue;
                    }
                    if (e.Args[i].Equals("-r", StringComparison.OrdinalIgnoreCase) ||
                        e.Args[i].Equals("--rar", StringComparison.OrdinalIgnoreCase))
                    {
                        format = CompressionFormat.Rar;
                        continue;
                    }
                }
                if (path is null) return;
                var compressor = new Compressor(path);
                compressor.Compress(format ?? CompressionFormat.Zip);
            }
            else
            {
                base.OnStartup(e);
                ProfileOptimization.SetProfileRoot(GetFolderPath(SpecialFolder.LocalApplicationData));
                ProfileOptimization.StartProfile("Startup.Profile");
            }
        }
    }
}
