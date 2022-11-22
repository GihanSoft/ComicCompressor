using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using ComicC.Logics;

using Microsoft.WindowsAPICodePack.Dialogs;

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
        HashSet<string> existingChapters = new(Chapters.Select(c => c.Path));

        foreach (var item in chapterPathes)
        {
            if (!existingChapters.Contains(item.ToLower()))
            {
                Chapters.Add(new ChapterVM { Path = item });
                await Dispatcher.Yield();
            }
        }
    }

    private void Item_DeleteClick(object sender, RoutedEventArgs e)
    {
        var item = (ChapterItem)sender;
        var chapterVM = (ChapterVM)item.DataContext;
        _ = Chapters.Remove(chapterVM);
    }

    private async void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        BtnStart.IsEnabled = false;
        BtnAdd.IsEnabled = false;
        ChkRemove.IsEnabled = false;
        GrdDrop.AllowDrop = false;

        await Dispatcher.Yield();

        try
        {
            await ComicCompressorEngine.CompressComicsAsync(Chapters, ChkRemove.IsChecked ?? false, null)
                .ConfigureAwait(true);
        }
        finally
        {
            BtnStart.IsEnabled = true;
            BtnAdd.IsEnabled = true;
            ChkRemove.IsEnabled = true;
            GrdDrop.AllowDrop = true;
        }
    }

    private void Grid_DragEnter(object sender, DragEventArgs e)
    {
        _ = ((Grid)sender).CaptureMouse();
        BrDrop.Visibility = Visibility.Visible;
    }

    private void Grid_DragLeave(object sender, DragEventArgs e)
    {
        ((Grid)sender).ReleaseMouseCapture();
        BrDrop.Visibility = Visibility.Collapsed;
    }

    private async void Grid_Drop(object sender, DragEventArgs e)
    {
        Grid_DragLeave(sender, e);

        var folders = ((string[])e.Data.GetData(DataFormats.FileDrop))
            .Where(f => File.GetAttributes(f).HasFlag(FileAttributes.Directory))
            .ToList();

        if (folders.Count > 0)
        {
            await AddItemsAsync(folders).ConfigureAwait(false);
        }
    }

    private void Border_MouseUp(object sender, MouseButtonEventArgs e)
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
