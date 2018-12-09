using System.Windows;
using System.Windows.Controls;

using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace Gihan.Wpf
{
    /// <summary>
    /// Interaction logic for FolderSelector.xaml
    /// </summary>
    public partial class FolderSelector : UserControl
    {
        public TextBox TextBox { get; }
        public Button Button { get; }
        public string DialogTitle { get; set; }

        public FolderSelector()
        {
            InitializeComponent();
            TextBox = TxtPath;
            Button = BtnBrowse;
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            //todo make it standard folder select
            var dialog = new FolderBrowserDialog
            {
                Description = DialogTitle,
            };
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TxtPath.Text = dialog.SelectedPath;
            }
        }
    }
}
