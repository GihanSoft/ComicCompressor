using System.Diagnostics;
using System.Windows.Navigation;

namespace ComicC;

/// <summary>
/// Interaction logic for InfoWin.xaml
/// </summary>
public partial class InfoWin
{
    public InfoWin()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        ProcessStartInfo startInfo = new(e.Uri.AbsoluteUri)
        {
            UseShellExecute = true,
        };
        _ = Process.Start(startInfo);
    }
}
