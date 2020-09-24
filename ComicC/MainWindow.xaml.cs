using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ComicC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string[] chapters) : this()
        {
            foreach (var item in chapters)
            {
                if (SpChapters.Children.Cast<ChapterItem>().Select(c => c.ChapterPath).Contains(item))
                {
                    continue;
                }
                var chapterItem = new ChapterItem(item);
                SpChapters.Children.Add(chapterItem);
                chapterItem.DeleteClicked += (s, eventArg) =>
                {
                    SpChapters.Children.Remove(s as UIElement);
                };
            }
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (var item in dialog.FileNames)
                {
                    if (SpChapters.Children.Cast<ChapterItem>().Select(c => c.ChapterPath).Contains(item))
                    {
                        continue;
                    }
                    var chapterItem = new ChapterItem(item);
                    SpChapters.Children.Add(chapterItem);
                    chapterItem.DeleteClicked += (s, eventArg) =>
                    {
                        SpChapters.Children.Remove(s as UIElement);
                    };
                    await Task.Delay(10).ConfigureAwait(false);
                }
            }
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in SpChapters.Children.Cast<ChapterItem>())
            {
                item.Background = new SolidColorBrush(Colors.LightYellow);
                await Task.Delay(10).ConfigureAwait(false);
                try
                {
                    ZipFile.CreateFromDirectory(
                        item.ChapterPath,
                        Path.Combine(
                            Path.GetDirectoryName(item.ChapterPath),
                            Path.GetFileName(item.ChapterPath)) + ".zip",
                        CompressionLevel.NoCompression,
                        false);
                    if (ChkRemove.IsChecked ?? false)
                    {
                        Directory.Delete(item.ChapterPath, true);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, err.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    item.Background = new SolidColorBrush(Colors.Red);
                }
                item.Background = new SolidColorBrush(Colors.LightGreen);
                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            (sender as Grid).CaptureMouse();
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
                foreach (var item in files)
                {
                    if (SpChapters.Children.Cast<ChapterItem>().Select(c => c.ChapterPath).Contains(item))
                        continue;
                    var chapterItem = new ChapterItem(item);
                    SpChapters.Children.Add(chapterItem);
                    chapterItem.DeleteClicked += (s, eventArg) =>
                    {
                        SpChapters.Children.Remove(s as UIElement);
                    };
                    await Task.Delay(10).ConfigureAwait(false);
                }
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
            new InfoWin().ShowDialog();
        }
    }
}
