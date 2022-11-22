using System;
using System.Threading.Tasks;

namespace ComicC;

/// <summary>
/// Interaction logic for SplashWin.xaml
/// </summary>
public partial class SplashWin
{
    private readonly DateTime startMoment = DateTime.Now;
    private readonly TimeSpan minimumViewDuration = TimeSpan.FromSeconds(2);

    public SplashWin()
    {
        DataContext = this;
        InitializeComponent();
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

        Close();
    }
}
