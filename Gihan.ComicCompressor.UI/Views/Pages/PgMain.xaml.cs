using Gihan.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gihan.ComicCompressor.Views.Pages
{
    /// <summary>
    /// Interaction logic for PgMain.xaml
    /// </summary>
    public partial class PgMain : Page
    {
        public PgMain()
        {
            InitializeComponent();
            CboFormat.ItemsSource = Enum.GetValues(typeof(CompressionFormat));
        }

        private async void BtnStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Compressor compressor;
            if (SpToCompressList.Children.Count == 0)
            {
                compressor = new Compressor(Fs.TextBox.Text);
            }
            else
            {
                var list = new List<DirectoryInfo>();
                foreach (DoubleEntry item in SpToCompressList.Children)
                {
                    if (item.IsChecked ?? false)
                    {
                        list.Add(new DirectoryInfo((string)item.Data));
                    }
                }
                compressor = new Compressor(list);
            }
            var format = (CompressionFormat)CboFormat.SelectedItem;

            Pr.Visibility = Visibility.Visible;
            IsEnabled = false;
            await Task.Run(() =>
            {
                compressor.Compress(format);
            });
            IsEnabled = true;
            Pr.Visibility = Visibility.Collapsed;
        }

        private void BtnFetch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Fs.TextBox.Text)) return;
            var dir = new DirectoryInfo(Fs.TextBox.Text);
            var items = dir.EnumerateDirectories().
                Where(d => (d.Attributes & FileAttributes.System) == 0);
            SpToCompressList.Children.Clear();
            foreach (var item in items)
            {
                var doubleEntry = new DoubleEntry();
                doubleEntry.Grid.ColumnDefinitions[0].Width = new GridLength(2, GridUnitType.Star);
                doubleEntry.TextBox1.IsReadOnly = true;
                doubleEntry.TextBox2.IsReadOnly = true;
                doubleEntry.MiddleBorder.Child = new TextBox { Text = "=>", IsReadOnly = true };
                doubleEntry.TextBox1.Text = item.FullName;
                doubleEntry.TextBox2.Text = Path.GetFileNameWithoutExtension(item.Name) +
                    "." + (CompressionFormat)CboFormat.SelectedItem;
                doubleEntry.CheckBox.IsChecked = true;
                doubleEntry.Data = item.FullName;
                SpToCompressList.Children.Add(doubleEntry);
            }
        }

        private void CboFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (DoubleEntry doubleEntry in SpToCompressList.Children)
            {
                doubleEntry.TextBox2.Text = Path.GetFileNameWithoutExtension(doubleEntry.TextBox1.Text) +
                     "." + (CompressionFormat)CboFormat.SelectedItem;
            }
        }

        private bool IsDragedAFolder(IDataObject data)
        {
            if (!data.GetDataPresent(DataFormats.FileDrop)) return false;

            var fileNames = (string[])data.GetData(DataFormats.FileDrop);
            foreach (var file in fileNames)
            {
                if (!Directory.Exists(file)) return false;
            }
            return true;
        }

        private void ScrollViewer_Drop(object sender, DragEventArgs e)
        {
            var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in fileNames)
            {
                if (!Directory.Exists(file)) continue;
                var doubleEntry = new DoubleEntry();
                doubleEntry.Grid.ColumnDefinitions[0].Width = new GridLength(2, GridUnitType.Star);
                doubleEntry.TextBox1.IsReadOnly = true;
                doubleEntry.TextBox2.IsReadOnly = true;
                doubleEntry.MiddleBorder.Child = new TextBox { Text = "=>", IsReadOnly = true };
                doubleEntry.TextBox1.Text = file;
                doubleEntry.TextBox2.Text = Path.GetFileNameWithoutExtension(file) +
                    "." + (CompressionFormat)CboFormat.SelectedItem;
                doubleEntry.CheckBox.IsChecked = true;
                doubleEntry.Data = file;
                SpToCompressList.Children.Add(doubleEntry);
            }
        }

        bool x = true;

        private void ScrollViewer_DragEnter(object sender, DragEventArgs e)
        {
            if (!IsDragedAFolder(e.Data))
            {
                (sender as UIElement).AllowDrop = false;
            }
            x = false;
        }

        private void Border_DragEnter(object sender, DragEventArgs e)
        {
            if (x)
            {
                Sv.AllowDrop = true;
            }
            x = true;
        }
    }
}
