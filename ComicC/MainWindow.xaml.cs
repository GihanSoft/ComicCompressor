using Microsoft.WindowsAPICodePack.Dialogs;
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

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = true
            };
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (var item in dialog.FileNames)
                {
                    var chapterItem = new ChapterItem(item);
                    SpChapters.Children.Add(chapterItem);
                    chapterItem.DeleteClicked += (s, eventArg) =>
                    {
                        SpChapters.Children.Remove(s as UIElement);
                    };
                    await Task.Delay(10);
                }
            }
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in SpChapters.Children.Cast<ChapterItem>())
            {
                item.Background = new SolidColorBrush(Colors.LightYellow);
                await Task.Delay(100);
                try
                {
                    ZipFile.CreateFromDirectory(
                        item.ChapterPath,
                        Path.Combine(
                            Path.GetDirectoryName(item.ChapterPath),
                            Path.GetFileName(item.ChapterPath)) + ".zip",
                        CompressionLevel.NoCompression,
                        false);
                }
                catch { }
                item.Background = new SolidColorBrush(Colors.LightGreen);
                await Task.Delay(100);
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

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            Grid_DragLeave(sender, e);
            //todo
        }
    }
}
