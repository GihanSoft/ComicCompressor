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
            if (chapters is null || chapters.Length is 0)
            {
                throw new ArgumentNullException(nameof(chapters));
            }

            foreach (var item in chapters)
            {
                if (SpChapters.Children.Cast<ChapterItem>().Select(c => c.ChapterPath).Contains(item))
                {
                    continue;
                }

                var chapterItem = new ChapterItem(item);
                _ = SpChapters.Children.Add(chapterItem);
                chapterItem.DelClicked += (s, eventArg) =>
                {
                    SpChapters.Children.Remove(s as UIElement);
                };
            }
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (var item in dialog.FileNames)
                {
                    var duplicate = Dispatcher.Invoke(() =>
                        SpChapters.Children.Cast<ChapterItem>().Select(c => c.ChapterPath).Contains(item));
                    if (duplicate)
                    {
                        continue;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        var chapterItem = new ChapterItem(item);
                        _ = SpChapters.Children.Add(chapterItem);
                        chapterItem.DelClicked += (s, eventArg) =>
                        {
                            SpChapters.Children.Remove(s as UIElement);
                        };
                    });
                    await Task.Delay(10).ConfigureAwait(false);
                }
            }
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                BtnStart.IsEnabled = false;
                BtnAdd.IsEnabled = false;
                ChkRemove.IsEnabled = false;
                GrdDrop.AllowDrop = false;
            });
            await Task.Delay(10).ConfigureAwait(false);

            var chaptersCount = Dispatcher.Invoke(() => SpChapters.Children.Count);
            for (int i = 0; i < chaptersCount; i++)
            {
                var item = Dispatcher.Invoke(() => SpChapters.Children[i] as ChapterItem);
                Dispatcher.Invoke(() =>
                {
                    item.Background = new SolidColorBrush(Colors.LightYellow);
                    item.IsBtnDelEnabled = false;
                });
                await Task.Delay(10).ConfigureAwait(false);
                try
                {
                    var chapterPath = Dispatcher.Invoke(() => item.ChapterPath);
                    ZipFile.CreateFromDirectory(
                        chapterPath,
                        Path.Combine(
                            Path.GetDirectoryName(chapterPath),
                            Path.GetFileName(chapterPath)) + ".cbz",
                        CompressionLevel.NoCompression,
                        false);
                    if (Dispatcher.Invoke(() => ChkRemove.IsChecked ?? false))
                    {
                        Directory.Delete(chapterPath, true);
                    }

                    Dispatcher.Invoke(() =>
                    {
                        item.Background = new SolidColorBrush(Colors.LightGreen);
                        item.IsBtnDelEnabled = true;
                    });
                }
                catch (Exception err) when (err.Message?.Length is > 0)
                {
                    _ = MessageBox.Show(err.Message, err.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    _ = Dispatcher.Invoke(() => item.Background = new SolidColorBrush(Colors.Red));
                }

                await Task.Delay(10).ConfigureAwait(false);
            }

            Dispatcher.Invoke(() =>
            {
                BtnStart.IsEnabled = true;
                BtnAdd.IsEnabled = true;
                ChkRemove.IsEnabled = true;
                GrdDrop.AllowDrop = true;
            });
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
            Dispatcher.Invoke(() => Grid_DragLeave(sender, e));
            var files = e.Data.GetData("FileDrop") as IEnumerable<string>;
            if (files.Any(f => File.GetAttributes(f).HasFlag(FileAttributes.Directory)))
            {
                files = files.Where(f => File.GetAttributes(f).HasFlag(FileAttributes.Directory));
                foreach (var item in files)
                {
                    var duplicate = Dispatcher.Invoke(() =>
                        SpChapters.Children.Cast<ChapterItem>().Select(c => c.ChapterPath).Contains(item));
                    if (duplicate)
                    {
                        continue;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        var chapterItem = new ChapterItem(item);
                        _ = SpChapters.Children.Add(chapterItem);
                        chapterItem.DelClicked += (s, eventArg) =>
                        {
                            SpChapters.Children.Remove(s as UIElement);
                        };
                    });
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
            _ = new InfoWin().ShowDialog();
        }
    }
}
