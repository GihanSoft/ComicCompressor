using System.Windows;
using System.Windows.Controls;

namespace ComicC;

/// <summary>
/// Interaction logic for ChapterItem.xaml
/// </summary>
public partial class ChapterItem : UserControl
{
    public ChapterItem()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        DelClicked?.Invoke(this, e);
    }

    public event RoutedEventHandler DelClicked;
}
