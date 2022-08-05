using System.Windows;
using System.Windows.Controls;

namespace ComicC;

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

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        DelClicked?.Invoke(this, e);
    }

    public bool IsBtnDelEnabled
    {
        get => BtnDel.IsEnabled;
        set => BtnDel.IsEnabled = value;
    }

    public event RoutedEventHandler DelClicked;
}
