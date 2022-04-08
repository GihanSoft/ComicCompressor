using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ComicC
{
    /// <summary>
    /// Interaction logic for SplashWin.xaml
    /// </summary>
    public partial class SplashWin : Window
    {
        public SplashWin()
        {
            InitializeComponent();
            var args = Environment.GetCommandLineArgs().ToList();
            if (args.Count > 1)
            {
                args.RemoveAt(0);
                Args = args.ToArray();
            }
        }

        private string[] Args { get; }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow win;
            if (Args != null)
            {
                var mangas = new List<string>();
                var chapters = new List<string>();
                var isM = false;
                for (int i = 0; i < Args.Length; i++)
                {
                    var arg = Args[i];
                    if (arg == "-m")
                    {
                        isM = true;
                        continue;
                    }

                    if (arg == "-c")
                    {
                        isM = false;
                        continue;
                    }

                    if (isM)
                    {
                        mangas.Add(arg);
                    }
                    else
                    {
                        chapters.Add(arg);
                    }
                }

                chapters.AddRange(mangas.SelectMany(m => Directory.GetDirectories(m)));
                win = new MainWindow(chapters.ToArray());
            }
            else
            {
                win = new MainWindow();
            }

            double val = 0;
            _ = Dispatcher.Invoke(() => val = pb.Value);
            double max = 0;
            _ = Dispatcher.Invoke(() => max = pb.Maximum);

            while (val < max)
            {
                await Task.Delay(10).ConfigureAwait(false);
                _ = Dispatcher.Invoke(() => pb.Value += 1);
                _ = Dispatcher.Invoke(() => val = pb.Value);
                _ = Dispatcher.Invoke(() => max = pb.Maximum);
            }

            await Fade().ConfigureAwait(false);
            Dispatcher.Invoke(() => win.Show());
        }

        private async Task Fade()
        {
            for (int i = 0; i < 50; i++)
            {
                Dispatcher.Invoke(() =>
                {
                    Opacity = (100 - (i * 2)) * 0.01;
                });

                await Task.Delay(10).ConfigureAwait(false);
            }

            Dispatcher.Invoke(() => Close());
        }
    }
}
