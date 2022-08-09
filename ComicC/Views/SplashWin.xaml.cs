using System;
using System.Threading.Tasks;
using System.Windows;

namespace ComicC;

/// <summary>
/// Interaction logic for SplashWin.xaml
/// </summary>
public partial class SplashWin : Window
{
    private readonly DateTime startMoment = DateTime.Now;
    private readonly TimeSpan minimumViewDuration = TimeSpan.FromSeconds(2);

    public SplashWin()
    {
        InitializeComponent();
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
    }

    public async Task CloseAsync()
    {
        var now = DateTime.Now;
        var viewDurrationRemained = minimumViewDuration - (now - startMoment);
        if (viewDurrationRemained.Ticks > 0)
        {
            await Task.Delay(viewDurrationRemained).ConfigureAwait(true);
        }

        for (int i = 0; i < 50; i++)
        {
            Opacity = (100 - (i * 2)) * 0.01;
            await Task.Delay(10).ConfigureAwait(true);
        }

        Dispatcher.Invoke(() => Close());
    }
}
