using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ComicC
{
    /// <summary>
    /// Interaction logic for SplashWin.xaml
    /// </summary>
    public partial class SplashWin : Window
    {
        public SplashWin()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var win = new MainWindow();

            for(pb.Value = 1; pb.Value < pb.Maximum; pb.Value += 1)
                await Task.Delay(10);

            await Fade();
            win.Show();
        }

        private async Task Fade()
        {
            for (int i = 0; i < 50; i++)
            {
                Opacity = (100 - i * 2) * 0.01;
                await Task.Delay(10);
            }
            Close();
        }
    }
}
