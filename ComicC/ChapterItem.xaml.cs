using System.Windows;
using System.Windows.Controls;

namespace ComicC
{
    /// <summary>
    /// Interaction logic for ChapterItem.xaml
    /// </summary>
    public partial class ChapterItem : UserControl
    {
        public ChapterItem(string chapterPath)
        {
            InitializeComponent();
            tb.Text = chapterPath;
            ChapterPath = chapterPath;
            ToolTip = chapterPath;
        }

        public string ChapterPath { get; }

        public event RoutedEventHandler DeleteClicked;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DeleteClicked?.Invoke(this, e);
        }
    }
}
