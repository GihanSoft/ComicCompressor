using System;
using System.Windows.Controls;

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

        private void BtnStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var compressor = Compressor.GetCompressor((CompressionFormat)CboFormat.SelectedItem, Fs.TextBox.Text);
            compressor.Compress();
        }
    }
}
