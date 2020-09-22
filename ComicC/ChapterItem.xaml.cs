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
        }

        public string ChapterPath { get; }
    }
}
