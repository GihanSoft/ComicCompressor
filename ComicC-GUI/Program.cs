using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using ComicC.Logics;

namespace ComicC;

public static class Program
{
    [DllImport("Kernel32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern bool AttachConsole(int processId);

    [DllImport("Kernel32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern bool FreeConsole();

    [STAThread]
    public static void Main(string[] args)
    {
        _ = AttachConsole(-1);
        try
        {
            var mangasOption = new Option<string[]>(new[] { "--manga", "-m" }) { AllowMultipleArgumentsPerToken = true }
                .LegalFilePathsOnly();
            var chaptersOption = new Option<string[]>(new[] { "--chapter", "-c" }) { AllowMultipleArgumentsPerToken = true }
                .LegalFilePathsOnly();

            var removeSourceOption = new Option<bool>(new[] { "--remove-source", "-r" });

            Command compressCommand = new("compress")
        {
            mangasOption,
            chaptersOption,
            removeSourceOption,
        };

            compressCommand.SetHandler(HandleCompressCommand, mangasOption, chaptersOption, removeSourceOption);

            RootCommand rootCommand = new()
        {
            mangasOption,
            chaptersOption,

            compressCommand,
        };

            rootCommand.SetHandler(MainSync, mangasOption, chaptersOption);

            _ = rootCommand.Invoke(args);
        }
        finally
        {
            _ = FreeConsole();
        }
    }

    private static async Task HandleCompressCommand(string[] mangas, string[] chapters, bool removeSource)
    {
        Console.WriteLine();

        var mangaChapters = mangas.SelectMany(m => Directory.EnumerateDirectories(m));
        var chaptersData = mangaChapters.Concat(chapters)
            .Select(p => new ChapterVM() { Path = p })
            .ToArray();

        Progress<ChapterVM> progress = new();
        progress.ProgressChanged += static (sender, report) =>
        {
            var defaultBgColor = Console.BackgroundColor;
            var defaultFgColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = report.Status switch
            {
                ChapterVM.ChapterStatus.Succeed => ConsoleColor.DarkGreen,
                ChapterVM.ChapterStatus.Failed => ConsoleColor.DarkRed,
                _ => ConsoleColor.Black,
            };
            Console.WriteLine(report.Path);

            Console.BackgroundColor = defaultBgColor;
            Console.ForegroundColor = defaultFgColor;
        };

        await ComicCompressorEngine.CompressComicsAsync(chaptersData, removeSource, progress)
            .ConfigureAwait(false);
    }

    private static void MainSync(string[] mangas, string[] chapters)
    {
        try
        {
            App app = new()
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown,
            };
            app.DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskCompletionSource<bool> mainEndTaskSignal = new();
            var mangaChapters = mangas.SelectMany(m => Directory.EnumerateDirectories(m));
            chapters = mangaChapters.Concat(chapters).ToArray();
            var mainAsyncTask = MainAsync(chapters, mainEndTaskSignal.Task);

            SplashWin splashWin = new();
            var exitCode = app.Run(splashWin);

            mainEndTaskSignal.SetResult(true);
            mainAsyncTask.Wait();
            Environment.ExitCode = exitCode;
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }

    private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        _ = MessageBox.Show(e.Exception.Message, e.Exception.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private static async Task MainAsync(string[] chapters, Task mainEndSignal)
    {
        await Task.Yield();

        await App.Current.Dispatcher.InvokeAsync(() => App.Current.InitializeComponent());
        await App.Current.Dispatcher.InvokeAsync(async () =>
        {
            var splashWin = App.Current.MainWindow as SplashWin ??
                throw new InvalidCastException($"main window is not {nameof(SplashWin)}");
            await splashWin.CloseAsync().ConfigureAwait(true);
        }).Task.Unwrap().ConfigureAwait(false);

        await App.Current.Dispatcher.InvokeAsync(() =>
        {
            MainWindow win = new();
            _ = win.AddItemsAsync(chapters);

            App.Current.MainWindow = win;
            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            win.Show();
            _ = win.Activate();
        });

        await mainEndSignal.ConfigureAwait(false);
    }
}
