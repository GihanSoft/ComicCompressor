using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ComicC;

public static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        _ = args ?? throw new ArgumentNullException(nameof(args));

        try
        {
            App app = new()
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown,
            };
            app.DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskCompletionSource<bool> mainEndTaskSignal = new();
            var mainAsyncTask = MainAsync(args, mainEndTaskSignal.Task);

            SplashWin splashWin = new();
            var exitCode = app.Run(splashWin);

            mainEndTaskSignal.SetResult(true);
            mainAsyncTask.Wait();
            return exitCode;
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

    private static async Task MainAsync(string[] args, Task mainEndSignal)
    {
        await Task.Yield();
        var chaptersTask = GetChaptersAsync(args);

        await App.Current.Dispatcher.InvokeAsync(() => App.Current.InitializeComponent());
        await App.Current.Dispatcher.InvokeAsync(async () =>
        {
            var splashWin = App.Current.MainWindow as SplashWin ??
                throw new InvalidCastException($"main window is not {nameof(SplashWin)}");
            await splashWin.CloseAsync().ConfigureAwait(true);
        }).Task.Unwrap().ConfigureAwait(false);

        var chapters = args.Length == 0 ? null : await chaptersTask.ConfigureAwait(false);
        await App.Current.Dispatcher.InvokeAsync(() =>
        {
            var win = chapters is null ? new MainWindow() : new MainWindow(chapters);
            App.Current.MainWindow = win;
            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            win.Show();
            _ = win.Activate();
        });

        await mainEndSignal.ConfigureAwait(false);
    }

    private static async Task<string[]> GetChaptersAsync(string[] args)
    {
        Option<string[]> mangaOption = new(new[] { "--manga", "-m" })
        {
            AllowMultipleArgumentsPerToken = true,
        };
        Option<string[]> chapterOption = new(new[] { "--chapter", "-c" })
        {
            AllowMultipleArgumentsPerToken = true,
        };

        RootCommand rootCommand = new();
        rootCommand.AddOption(mangaOption);
        rootCommand.AddOption(chapterOption);
        var result = Array.Empty<string>();

        rootCommand.SetHandler((mangas, chapters) =>
        {
            var mangaChapters = mangas.SelectMany(m => Directory.GetDirectories(m));
            result = chapters.Concat(mangaChapters).ToArray();
        }, mangaOption, chapterOption);

        _ = await rootCommand.InvokeAsync(args).ConfigureAwait(false);
        return result;
    }
}
