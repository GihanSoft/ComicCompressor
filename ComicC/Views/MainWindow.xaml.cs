using ComicC.Logics;

using Microsoft.WindowsAPICodePack.Dialogs;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ComicC;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        Chapters = new();
        DataContext = this;
        InitializeComponent();
    }

    public ObservableCollection<ChapterVM> Chapters { get; }

    private async void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        using var dialog = new CommonOpenFileDialog
        {
            IsFolderPicker = true,
            Multiselect = true
        };
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            await AddItemsAsync(dialog.FileNames).ConfigureAwait(false);
        }
    }

    public async Task AddItemsAsync(IEnumerable<string> chapterPathes)
    {
        foreach (var item in chapterPathes)
        {
            var duplicate = Chapters.Any(c => c.Path == item);
            if (duplicate)
            {
                continue;
            }

            Chapters.Add(new ChapterVM { Path = item });
            await Dispatcher.Yield();
        }
    }

    private void Item_DeleteClick(object sender, RoutedEventArgs e)
    {
        var item = sender as ChapterItem ?? throw new InvalidCastException();
        var chapterVM = item.DataContext as ChapterVM ?? throw new InvalidCastException();
        _ = Chapters.Remove(chapterVM);
    }

    private async void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        BtnStart.IsEnabled = false;
        BtnAdd.IsEnabled = false;
        ChkRemove.IsEnabled = false;
        GrdDrop.AllowDrop = false;

        await Dispatcher.Yield();

        await ComicCompressorEngine.CompressComicsAsync(Chapters, ChkRemove.IsChecked ?? false, null)
            .ConfigureAwait(true);

        BtnStart.IsEnabled = true;
        BtnAdd.IsEnabled = true;
        ChkRemove.IsEnabled = true;
        GrdDrop.AllowDrop = true;
    }

    private void Grid_DragEnter(object sender, DragEventArgs e)
    {
        _ = (sender as Grid).CaptureMouse();
        BrDrop.Visibility = Visibility.Visible;
    }

    private void Grid_DragLeave(object sender, DragEventArgs e)
    {
        (sender as Grid).ReleaseMouseCapture();
        BrDrop.Visibility = Visibility.Collapsed;
    }

    private async void Grid_Drop(object sender, DragEventArgs e)
    {
        Grid_DragLeave(sender, e);

        var files = e.Data.GetData("FileDrop") as IEnumerable<string>;
        if (files.Any(f => File.GetAttributes(f).HasFlag(FileAttributes.Directory)))
        {
            files = files.Where(f => File.GetAttributes(f).HasFlag(FileAttributes.Directory));
            await AddItemsAsync(files).ConfigureAwait(false);
        }
    }

    private void Border_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        BrdMenu.Visibility = Visibility.Collapsed;
    }

    private void BtnMenu_Click(object sender, RoutedEventArgs e)
    {
        BrdMenu.Visibility =
            BrdMenu.Visibility == Visibility.Visible ?
            Visibility.Collapsed : Visibility.Visible;
    }

    private void BtnInfo_Click(object sender, RoutedEventArgs e)
    {
        _ = new InfoWin().ShowDialog();
    }
}
